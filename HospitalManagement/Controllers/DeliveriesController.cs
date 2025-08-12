using Hangfire;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{

    public class DeliveriesController : Controller
    {

        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly ReceivePescribedMedication _receiveMedication;

        public DeliveriesController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            QrCodeService qrCodeService,
            EmailService emailService,
            ReceivePescribedMedication receiveMedication)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _receiveMedication = receiveMedication;
        }
        

        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> Packages()
        {
            var packages = await _context.Packages
                .Include(p => p.DeliveryPersonnel)
                .Include(p => p.DeliveryRequest)
                .ThenInclude(p => p.MedicationPescription)
                .Include(p => p.CreatedBy)
                .Include(p => p.ModifiedBy)
                .ToListAsync();

            return View(packages);
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> MedicationDeliveryRequests()
        {
            var requests = await _context.DeliveryRequests
                .Include(i => i.MedicationPescription)
                   .ThenInclude(i => i.PrescribedMedication)
                .Include(i => i.Patient)
                .Include(i => i.ModifiedBy)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyDeliveryRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            var deliveryRequests = await _context.DeliveryRequests
                .Where(dr => dr.PatientId == user.Id)
                .Include(dr => dr.MedicationPescription)
                .Include(dr => dr.Patient)
                .ToListAsync();

            return View(deliveryRequests);
        }

        [HttpGet]
        [Authorize(Roles = "Delivery Personnel")]
        public async Task<IActionResult> PendingDeliveries()
        {
            var deliveryRequests = await _context.DeliveryRequests
                .Where(dr => dr.Status == DeliveryRequestStatus.Packaged ||
                 dr.Status == DeliveryRequestStatus.OnTheWay)
                .Include(dr => dr.Patient)
                .Include(dr => dr.DeliveryPackageItems)
                   .ThenInclude(dr => dr.Medication)
                .ToListAsync();

            return View(deliveryRequests);
        }

        [HttpGet]
        [Authorize(Roles = "Delivery Personnel")]
        public async Task<IActionResult> PreviousDeliveries()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Receptionist, System Administrator")]
        public async Task<IActionResult> Deliveries()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewDeliveryRequest(NewDeliveryRequestViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var pescription = await _context.MedicationPescription
                    .Include(mp => mp.PrescribedMedication)
                    .Where(mp => mp.MedicationPescriptionId == viewModel.PescriptionId)
                    .FirstOrDefaultAsync();

                var newDeliveryRequest = new DeliveryRequest
                {
                    PatientId = user.Id,
                    Address = $"{viewModel.Street}, {viewModel.City}, {viewModel.Province}, {viewModel.PostalCode}, {viewModel.Country}",
                    MedicationPescriptionId = viewModel.PescriptionId,
                    LastUpdatedAt = DateTime.Now,
                    Status = DeliveryRequestStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                _context.DeliveryRequests.Add(newDeliveryRequest);
                await _context.SaveChangesAsync();

                if (pescription?.PrescribedMedication != null)
                {
                    foreach (var medication in pescription.PrescribedMedication)
                    {
                        var packageItem = new DeliveryPackageItem
                        {
                            DeliveryRequestId = newDeliveryRequest.DeliveryRequestId,
                            MedicationId = medication.MedicationId,
                            IsPackaged = false,
                            IsCollected = false
                        };
                        _context.DeliveryPackageItems.Add(packageItem);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = $"You have successfully requested your medication to be delivered to your house.";

                return RedirectToAction(nameof(MyDeliveryRequests));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to make a delivery request: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeliverMedication(int deliveryRequestId)
        {
            var deliveryRequest = await _context.DeliveryRequests
                .Where(dr => dr.DeliveryRequestId == deliveryRequestId)
                .FirstOrDefaultAsync();

            deliveryRequest.Status = DeliveryRequestStatus.OnTheWay;
            deliveryRequest.LastUpdatedAt = DateTime.Now;

            _context.Update(deliveryRequest);
            await _context.SaveChangesAsync();

            var package = await _context.Packages
                .Where(p => p.DeliveryRequestId == deliveryRequestId)
                .FirstOrDefaultAsync();

            var delivery = new Delivery
            {
                Address = deliveryRequest.Address,
                DeliveryDate = DateTime.Now,
                PackageId = package.PackageId,
                Status = DeliveryStatus.Started
            };

            var encryptedPackageId = _encryptionService.Encrypt(package.PackageId);

            BackgroundJob.Enqueue(() => _receiveMedication.NotifyPatientOnTheWayAsync(encryptedPackageId));

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ScanPackage(string packageId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var decryptedPackageId = _encryptionService.DecryptToInt(packageId);

            var package = await _context.Packages
                .Where(p => p.PackageId == decryptedPackageId && p.DeliveryRequest.Patient.Id == user.Id)
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return RedirectToAction("Home", "Error");
            }

            var viewModel = new ScanPackageViewModel
            {
                packageId = package.PackageId
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerifyScannedPackage(string qrCodeNumber, int packageId)
        {
            if (string.IsNullOrEmpty(qrCodeNumber))
                return Json(new { success = false, message = "Invalid QR code." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not authenticated." });

            var package = await _context.Packages
                .Include(p => p.DeliveryRequest)
                .FirstOrDefaultAsync(p => p.PackageNumber == qrCodeNumber);

            if (package == null)
                return Json(new { success = false, message = "Package not found." });

            if (package.PackageId != packageId)
                return Json(new { success = false, message = "The QR code you scanned is not supposed to be scanned using this scanner. Please check and use the corresponding scanner." });

            if (package.DeliveryRequest.PatientId != user.Id)
                return Json(new { success = false, message = "You do not have permission to receive this package." });

            if (package.DeliveryRequest.PatientId == user.Id && package.DeliveryRequest.Status == DeliveryRequestStatus.Collected)
                return Json(new { success = false, message = "This package was already collected. Please contact your pharmacist." });

            var encryptedPackageId = _encryptionService.Encrypt(package.PackageId);
            var redirectUrl = Url.Action("ReceiveMedication", "Deliveries", new { packageId = encryptedPackageId });

            return Json(new { success = true, redirectUrl });
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarkItemAsCollected(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not authenticated." });

            var item = await _context.DeliveryPackageItems
                .Include(dpi => dpi.DeliveryRequest)
                .FirstOrDefaultAsync(dpi => dpi.DeliveryPackageItemId == itemId);

            if (item == null)
                return Json(new { success = false, message = "Item not found." });

            if (item.DeliveryRequest.PatientId != user.Id)
                return Json(new { success = false, message = "Unauthorized." });

            if (!item.IsCollected)
            {
                item.IsCollected = true;
                item.CollectionAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PackageDetails(string packageId)
        {
            var decryptedPackageId = _encryptionService.DecryptToInt(packageId);

            var package = await _context.Packages
                .Include(p => p.DeliveryPersonnel)
                .Include(p => p.DeliveryRequest)
                    .ThenInclude(dr => dr.MedicationPescription)
                .Include(p => p.DeliveryRequest)
                    .ThenInclude(dr => dr.DeliveryPackageItems)
                        .ThenInclude(dpi => dpi.Medication)
                .Where(p => p.PackageId == decryptedPackageId)
                .FirstOrDefaultAsync();

            if (package == null)
                return NotFound("Package not found.");

            return View(package);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReceiveMedication(string packageId)
        {
            var decryptedPackageId = _encryptionService.DecryptToInt(packageId);

            var package = await _context.Packages
                .Include(p => p.DeliveryPersonnel)
                .Include(p => p.DeliveryRequest)
                    .ThenInclude(dr => dr.MedicationPescription) 
                .Include(p => p.DeliveryRequest)
                    .ThenInclude(dr => dr.DeliveryPackageItems)
                        .ThenInclude(dpi => dpi.Medication)
                .Where(p => p.PackageId == decryptedPackageId)
                .FirstOrDefaultAsync();

            if (package == null)
                return NotFound("Package not found.");

            return View(package);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CollectPackage(int packageId)
        {
            var package = await _context.Packages
                .Where(p => p.PackageId == packageId)
                .Include(p => p.DeliveryPersonnel)
                .Include(p => p.DeliveryRequest)
                .FirstOrDefaultAsync();

            var deliveryRequest = await _context.DeliveryRequests
                .Where(dr => dr.DeliveryRequestId == package.DeliveryRequestId)
                .FirstOrDefaultAsync();


            deliveryRequest.Status = DeliveryRequestStatus.Collected;

            _context.Update(deliveryRequest);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"You have successfully collected your medication package {package.PackageNumber}. Your delivery request has been completed. " +
                $"Please follow all your doctors instructions on taking your medication. " +
                $"Maintain your health.";

            return RedirectToAction(nameof(MyDeliveryRequests));
        }


        [HttpPost]
        public async Task<IActionResult> PackageMedicationItem(int packageItemId)
        {
            var item = await _context.DeliveryPackageItems.FindAsync(packageItemId);
            if (item == null)
                return NotFound();

            item.IsPackaged = true;
            item.PackagedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> PackageMedication(string requestId)
        {
            var decryptedRequestId = _encryptionService.DecryptToInt(requestId);

            var deliveryRequest = await _context.DeliveryRequests
                .Include(dr => dr.Patient)
                .Include(dr => dr.MedicationPescription)
                .Include(dr => dr.DeliveryPackageItems)
                    .ThenInclude(dpi => dpi.Medication)
                .FirstOrDefaultAsync(dr => dr.DeliveryRequestId == decryptedRequestId);

            return View(deliveryRequest);
        }


        [HttpPost]
        public async Task<IActionResult> PackageMedicationPost(string requestId)
        {
            var user = await _userManager.GetUserAsync(User);

            var decryptedRequestId = _encryptionService.DecryptToInt(requestId);

            var deliveryRequest = await _context.DeliveryRequests
                .Include(dr => dr.Patient)
                .Include(dr => dr.MedicationPescription)
                .Include(dr => dr.DeliveryPackageItems)
                    .ThenInclude(dpi => dpi.Medication)
                .FirstOrDefaultAsync(dr => dr.DeliveryRequestId == decryptedRequestId);

            if (deliveryRequest == null)
            {
                return NotFound();
            }

            deliveryRequest.Status = DeliveryRequestStatus.Packaged;
            deliveryRequest.LastUpdatedAt = DateTime.UtcNow;

            var availableDriver = await _context.DeliveryPersonnels
                .Where(dp => dp.IsAvailable)
                .FirstOrDefaultAsync();

            if (availableDriver == null)
            {
                TempData["Message"] = "No available delivery personnel found. Please try again later.";
                return RedirectToAction(nameof(MedicationDeliveryRequests));
            }

            var packageNumber = await GeneratePackageNumber();

            var package = new Package
            {
                PackageNumber = packageNumber,
                DeliveryRequestId = deliveryRequest.DeliveryRequestId,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedById = user.Id,
                CreatedAt = DateTime.UtcNow,
                DeliveryPersonnelId = "22305c20-9172-40a4-b042-eb344a1dcee0",
                UpdatedById = user.Id
            };

            _context.Add(package);
            await _context.SaveChangesAsync();

            package.QrCodeImage = _qrCodeService.GenerateQrCode(packageNumber);
            _context.Update(package);
            _context.Update(deliveryRequest);
            await _context.SaveChangesAsync();

            TempData["Message"] = "You have successfully completed packaging the medication delivery request. The request has been assigned to the available delivery personnel.";

            return RedirectToAction(nameof(MedicationDeliveryRequests));
        }

        private async Task<string> GeneratePackageNumber()
        {

            const string numbers = "0123456789";
            const string fineLetters = "PCGMEDPR-";

            var random = new Random();
            var randomNumbers = new string(Enumerable.Repeat(numbers, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{fineLetters}{randomNumbers}";
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeliverPackage(string deliveryRequestId)
        {
            return View();
        }
    }
}
