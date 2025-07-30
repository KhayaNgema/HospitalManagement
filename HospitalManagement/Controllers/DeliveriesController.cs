using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Cryptography.Xml;

namespace HospitalManagement.Controllers
{

    public class DeliveriesController : Controller
    {

        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public DeliveriesController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            QrCodeService qrCodeService,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyDeliveryRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            var deliveryRequests = await _context.DeliveryRequests
                .Where(dr => dr.PatientId == user.Id)
                .ToListAsync();

            return View(deliveryRequests);
        }

        [HttpGet]
        [Authorize(Roles = "Delivery Personnel")]
        public async Task<IActionResult> PendingDeliveries()
        {
            return View();
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

                _context.Add(newDeliveryRequest);
                await _context.SaveChangesAsync();

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


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CollectPackage(string packageId)
        {
            var decryptedPackageId = _encryptionService.DecryptToInt(packageId);

            var package = await _context.Packages
                .Include(p => p.DeliveryRequest)
                .Where(p => p.PackageId == decryptedPackageId)
                .FirstOrDefaultAsync();

            var viewModel = new CollectPackageViewModel
            {

            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CollectPackage(CollectPackageViewModel viewModel)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PackageMedication(string deliveryRequestId)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeliverPackage(string deliveryRequestId)
        {
            return View();
        }
    }
}
