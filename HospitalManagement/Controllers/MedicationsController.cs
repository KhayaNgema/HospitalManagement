using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
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

        public MedicationsController(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            SignInManager<UserBaseModel> signInManager,
            IEmailSender emailSender,
            FileUploadService fileUploadService,
            RoleManager<IdentityRole> roleManager,
            RandomPasswordGeneratorService passwordGenerator,
            EmailService emailService,
            HospitalManagementDbContext db,
            IActivityLogger activityLogger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _roleManager = roleManager;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _context = db;
            _activityLogger = activityLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Medications()
        {
            var medications = await _context.Medications
                .ToListAsync();

            return View(medications);
        }

        [HttpGet]
        public async Task<IActionResult> NewMedication()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewMedication(MedicationViewModel viewModel, IFormFile MedicationImages)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

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
                    
                };

                if (viewModel.MedicationImages != null && viewModel.MedicationImages.Length > 0)
                {
                    var playerProfilePicturePath = await _fileUploadService.UploadFileAsync(viewModel.MedicationImages);
                    menuItem.MedicationImage= playerProfilePicturePath;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added {viewModel.MedicationName} as your new hospital medication";

                return RedirectToAction(nameof(Medications));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add new menu item: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });

                return View(viewModel);
            }
        }
    }
}
