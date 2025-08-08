using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace HospitalManagement.Controllers

{
    public class MedicationsController : Controller
    {
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly FileUploadService _fileUploadService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RandomPasswordGeneratorService _passwordGenerator;
        private readonly IEmailSender _emailSender;
        private readonly EmailService _emailService;
        private readonly HospitalManagementDbContext _context;
        private readonly IActivityLogger _activityLogger;
        private readonly OrderCalculationService _orderCalculationService;
        private readonly IEncryptionService _encryptionService;
        private readonly BarcodeService _barcodeService;

        public MedicationsController(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            SignInManager<UserBaseModel> signInManager,
            IEmailSender emailSender,
            FileUploadService fileUploadService,
            RoleManager<IdentityRole> roleManager,
            RandomPasswordGeneratorService passwordGenerator,
            EmailService emailService,
            IEncryptionService encryptionService,
            HospitalManagementDbContext db,
            IActivityLogger activityLogger,
            BarcodeService barcodeService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _encryptionService = encryptionService;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _roleManager = roleManager;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _context = db;
            _activityLogger = activityLogger;
            _barcodeService = barcodeService;
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> Inventory()
        {
            var inventory = await _context.MedicationInventory
                .Include(i => i.Medication)
                .OrderBy(i => i.Medication.MedicationName)
                .ToListAsync();

            return View(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> Reminders()
        {
            var user = await _userManager.GetUserAsync(User);

            var medications = await _context.MedicationPescription
                .Include(mp => mp.Admission).ThenInclude(a => a.Patient)
                .Include(mp => mp.Booking).ThenInclude(b => b.CreatedBy)
                .Where(mp =>
                    (mp.Admission != null && mp.Admission.Patient != null && mp.Admission.Patient.Id == user.Id) ||
                    (mp.Booking != null && mp.Booking.CreatedBy != null && mp.Booking.CreatedBy.Id == user.Id)
                )
                .Select(mp => mp.MedicationPescriptionId)
                .ToListAsync();

            var reminders = await _context.MedicationReminders
                .Where(r => medications.Contains(r.MedicationPescriptionId)
                            && r.Status == ReminderStatus.Sent
                            && r.ExpiryDate >= DateTime.Now)
                .ToListAsync();

            var deliveryRequests = new Dictionary<int, NewDeliveryRequestViewModel>();

            foreach (var reminder in reminders)
            {
                var pescription = await _context.MedicationPescription
                    .Where(mp => mp.MedicationPescriptionId == reminder.MedicationPescriptionId)
                    .Include(mp => mp.Admission).ThenInclude(a => a.Patient)
                    .Include(mp => mp.Booking).ThenInclude(b => b.Patient)
                    .FirstOrDefaultAsync();

                var address = pescription?.Admission?.Patient?.Address ?? pescription?.Booking?.Patient?.Address;

                var parts = address?.Split(',')?.Select(p => p.Trim()).ToArray();

                if (parts != null && parts.Length >= 5)
                {
                    if (Enum.TryParse<Province>(parts[2].Replace(" ", "").Replace("-", ""), ignoreCase: true, out var prov))
                    {
                        var deliveryRequestVM = new NewDeliveryRequestViewModel
                        {
                            PescriptionId = pescription.MedicationPescriptionId,
                            Street = parts[0],
                            City = parts[1],
                            Province = prov,
                            PostalCode = parts[3],
                            Country = parts[4],
                        };

                        deliveryRequests[pescription.MedicationPescriptionId] = deliveryRequestVM;
                    }
                }
            }

            var viewModel = new MedicationRemindersWithDeliveryViewModel
            {
                Reminders = reminders,
                DeliveryRequests = deliveryRequests
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> CollectMedication()
        {
            var user = await _userManager.GetUserAsync(User);

            var medications = await _context.MedicationPescription
                .Include(mp => mp.PrescribedMedication)
                .Include(mp => mp.Booking).ThenInclude(b => b.CreatedBy)
                .Include(mp => mp.Admission).ThenInclude(a => a.Patient)
                .Include(mp => mp.Admission).ThenInclude(a => a.Booking)
                .Where(mp =>
                    (mp.Admission != null && mp.Admission.Patient != null && mp.Admission.Patient.Id == user.Id) ||
                    (mp.Booking != null && mp.Booking.CreatedBy != null && mp.Booking.CreatedBy.Id == user.Id)
                )
                .ToListAsync();

            var medicationIds = medications.Select(m => m.MedicationPescriptionId).ToList();

            var reminders = await _context.MedicationReminders
                .Where(r => medicationIds.Contains(r.MedicationPescriptionId)
                            && r.Status == ReminderStatus.Sent
                            && r.ExpiryDate >= DateTime.Now)
                .ToListAsync();

            var viewModel = new CollectMedicationViewModel
            {
                Medications = medications,
                Reminders = reminders
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> MedicationPescriptionRequests()
        {
            var medications = await _context.MedicationPescription
                .Include(mp => mp.PrescribedMedication)
                .Include(mp => mp.Booking)
                .ThenInclude(mp => mp.CreatedBy)
                .Include(mp => mp.Admission)
                .ThenInclude(mp => mp.Booking)
                .Include(mp => mp.CreatedBy)
                .Include(mp => mp.ModifiedBy)
                .Where(mp => mp.Status == MedicationPescriptionStatus.Pending ||
                mp.Status == MedicationPescriptionStatus.Collecting)
                .OrderByDescending(mp => mp.CreatedAt)
                .ToListAsync();

            return View(medications);
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> PreviousMedicationRequests()
        {
            var medications = await _context.MedicationPescription
                .Include(mp => mp.PrescribedMedication)
                .Include(mp => mp.Booking)
                .ThenInclude(mp => mp.CreatedBy)
                .Include(mp => mp.Admission)
                .ThenInclude(mp => mp.Booking)
                .Include(mp => mp.CreatedBy)
                .Include(mp => mp.ModifiedBy)
                .Where(mp => mp.Status == MedicationPescriptionStatus.Collected)
                .OrderByDescending(mp => mp.CreatedAt)
                .ToListAsync();

            return View(medications);
        }

        [Authorize(Roles = "Supplier Administrator, Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> Medications()
        {
            var medications = await _context.Medications
                .Include(m => m.Category)
                .ToListAsync();

            return View(medications);
        }

        [Authorize(Roles = "Pharmacist, Doctor")]
        [HttpGet]
        public async Task<IActionResult> PescriptionRequest(string medicationPescriptionId)
        {
            var decryptedMedicationPescriptionId = _encryptionService.DecryptToInt(medicationPescriptionId);

            var pescriptionRequest = await _context.MedicationPescription
                .Where(pr => pr.MedicationPescriptionId == decryptedMedicationPescriptionId)
                .Include(pr => pr.Booking)
                    .ThenInclude(b => b.CreatedBy)
                .Include(pr => pr.Booking)
                    .ThenInclude(b => b.AssignedTo)
                .Include(pr => pr.Admission)
                    .ThenInclude(a => a.Patient)
                .Include(pr => pr.Admission)
                    .ThenInclude(a => a.CreatedBy)
                 .Include(pr => pr.PrescribedMedication)
                .FirstOrDefaultAsync();

            if (pescriptionRequest == null)
            {
                return NotFound();
            }

            var booking = pescriptionRequest.Booking;
            var admission = pescriptionRequest.Admission;

            var patient = booking?.CreatedBy ?? admission?.Patient;
            var doctor = booking?.AssignedTo ?? admission.CreatedBy;

            if (patient == null || doctor == null)
            {
                return BadRequest("Incomplete prescription data.");
            }

            var viewModel = new PescriptionRequestViewModel
            {
                AccessCode = pescriptionRequest.AccessCode,
                PatientFirstName = patient.FirstName,
                PatientLastName = patient.LastName,
                PatientIdNumber = patient.IdNumber,
                Email = patient.Email,
                ProfilePicture = patient.ProfilePicture,
                PhoneNumber = patient.PhoneNumber,
                DoctorFirstName = doctor.FirstName,
                DoctorLastName = doctor.LastName,
                DoctorEmail = doctor.Email,
                DoctorPhoneNumber = doctor.PhoneNumber,
                DoctorSpecialization = doctor.Specialization,
                LastCollectionDate = pescriptionRequest.LastCollectionDate,
                NextCollectionDate = pescriptionRequest.NextCollectionDate,
                PescribedMedication = pescriptionRequest.PrescribedMedication,
                PescriptionRequestId = decryptedMedicationPescriptionId,
                AdditionalNotes = pescriptionRequest.AdditionalNotes,
                PrescriptionType = pescriptionRequest.PrescriptionType,
                CollectAfterCount = pescriptionRequest.CollectAfterCount,
                CollectInterval = pescriptionRequest.CollectionInterval,
                CollectUntilDate = pescriptionRequest.ExpiresAt,
                DoctorDepartment = doctor.Department,
                Status = pescriptionRequest.Status,

            };

            return View(viewModel);
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> ReceiveStock()
        {
            return View();
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpPost]
        public async Task<IActionResult> ReceiveStockByBarcode([FromBody] BarcodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Barcode))
            {
                return Json(new { status = "error", message = "No barcode provided." });
            }

            var inventory = await _context.MedicationInventory
                .Include(i => i.Medication)
                .FirstOrDefaultAsync(i => i.Medication.BarcodeValue == request.Barcode);

            if (inventory == null)
            {
                return Json(new { status = "error", message = "Medication not found for this barcode." });
            }

            inventory.Quantity++;
            inventory.Availability = MedicationAvailability.Available;
            _context.Update(inventory);
            await _context.SaveChangesAsync();

            return Json(new
            {
                status = "success",
                medicationName = inventory.Medication.MedicationName,
                barcode = request.Barcode
            });
        }


        public class BarcodeRequest
        {
            public string Barcode { get; set; }
        }


        [Authorize(Roles = "Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> NewMedication()
        {

            var categories = await _context.MedicationCategories
                .ToListAsync();

            ViewBag.Categories = categories;

            return View();
        }

        [Authorize(Roles = "Supplier Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewMedication(MedicationViewModel viewModel, IFormFile MedicationImages)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var barcodeValue = new Random().Next(10000000, 99999999).ToString();


                string barcodeFileName = $"MED-{barcodeValue}.png";
                string barcodePath = await _barcodeService.GenerateAndSaveBarcodeAsync(barcodeValue, barcodeFileName);

                var menuItem = new Medication
                {
                    MedicationName = viewModel.MedicationName,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Manufacturer = viewModel.Manufacturer,
                    DosageForm = viewModel.DosageForm,
                    Strength = viewModel.Strength,
                    UnitOfMeasure = viewModel.UnitOfMeasure,
                    ExpiryDate = viewModel.ExpiryDate,
                    IsExpired = false,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedById = user.Id,
                    IsPrescriptionRequired = true,
                    CategoryId = viewModel.CategoryId,
                    BarcodeValue = barcodeValue,
                    BarcodeImage = barcodePath
                };

                if (viewModel.MedicationImages != null && viewModel.MedicationImages.Length > 0)
                {
                    var uploadedImagePath = await _fileUploadService.UploadFileAsync(viewModel.MedicationImages);
                    menuItem.MedicationImage = uploadedImagePath;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();

                var newInventory = new MedicationInventory
                {
                    MedicationId = menuItem.MedicationId,
                    Quantity = 0,
                    StockLevel = StockLevel.Critical,
                    Availability = MedicationAvailability.OutOfStock
                };

                _context.Add(newInventory);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added {viewModel.MedicationName} with barcode {barcodeValue}";

                return RedirectToAction(nameof(Medications));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add new medication: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }


        [Authorize(Roles = "Pharmacist, System Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int prescriptionRequestId, MedicationPescriptionStatus status)
        {
            var user = await _userManager.GetUserAsync(User);

            var medicationPrescription = await _context.MedicationPescription
                .Include(p => p.PrescribedMedication)
                .FirstOrDefaultAsync(p => p.MedicationPescriptionId == prescriptionRequestId);

            if (medicationPrescription == null)
            {
                TempData["Error"] = "Prescription request not found.";
                return RedirectToAction(nameof(NewMedication));
            }

            medicationPrescription.Status = status;
            medicationPrescription.LastUpdatedAt = DateTime.Now;
            medicationPrescription.UpdatedById = user.Id;

            if (status == MedicationPescriptionStatus.Collected)
            {
                foreach (var prescribed in medicationPrescription.PrescribedMedication)
                {
                    var inventory = await _context.MedicationInventory
                        .Include(i => i.Medication)
                        .FirstOrDefaultAsync(i => i.Medication.MedicationId == prescribed.MedicationId);

                    if (inventory == null) continue;

                    if (inventory.Quantity > 0)
                        inventory.Quantity--;

                    if (inventory.Quantity <= 0)
                    {
                        inventory.StockLevel = StockLevel.Critical;
                        inventory.Availability = MedicationAvailability.OutOfStock;
                    }
                    else if (inventory.Quantity <= 5)
                        inventory.StockLevel = StockLevel.Critical;
                    else if (inventory.Quantity <= 10)
                        inventory.StockLevel = StockLevel.Low;
                    else if (inventory.Quantity <= 25)
                        inventory.StockLevel = StockLevel.Moderate;
                    else
                        inventory.StockLevel = StockLevel.High;

                    if (inventory.Quantity > 0)
                        inventory.Availability = MedicationAvailability.Available;

                    _context.Update(inventory);

                    var usageLog = new MedicationUsageLog
                    {
                        MedicationId = prescribed.MedicationId,
                        MedicationPescriptionId = medicationPrescription.MedicationPescriptionId,
                        QuantityDispensed = 1,
                        DispensedById = user.Id,
                        DispensedOn = DateTime.Now,
                        Notes = "Medication dispensed from prescription request."
                    };
                    _context.MedicationUsageLogs.Add(usageLog);
                }
            }

            _context.Update(medicationPrescription);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"You have successfully updated the medication prescription request status to {medicationPrescription.Status}.";

            var encryptedId = _encryptionService.Encrypt(medicationPrescription.MedicationPescriptionId);
            return RedirectToAction(nameof(PescriptionRequest), new { medicationPescriptionId = encryptedId });
        }
    }
}
