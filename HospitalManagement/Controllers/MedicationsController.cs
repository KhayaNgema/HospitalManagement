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
            IActivityLogger activityLogger)
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


            return View(medications);
        }


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
                .OrderByDescending(mp => mp.CreatedAt)
                .ToListAsync();

            return View(medications);
        }


        [HttpGet]
        public async Task<IActionResult> Medications()
        {
            var medications = await _context.Medications
                .ToListAsync();

            return View(medications);
        }

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
                DoctorDepartment = doctor.Department
                
            };

            return View(viewModel);
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


        [Authorize(Roles = "Pharmacist, System Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusRedirect(int prescriptionRequestId, MedicationPescriptionStatus status)
        {
            var user = await _userManager.GetUserAsync(User);

            var medicationprescriptionRequest = await _context.MedicationPescription
                .Where(mpr => mpr.MedicationPescriptionId == prescriptionRequestId)
                .FirstOrDefaultAsync();


            if (medicationprescriptionRequest != null)
            {
                medicationprescriptionRequest.Status = status;
                medicationprescriptionRequest.LastUpdatedAt = DateTime.Now;
                medicationprescriptionRequest.UpdatedById = user.Id;

                _context.Update(medicationprescriptionRequest);
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = $"You have successfully updated the medication prescription request status to {status}.";

            var encryptedId = _encryptionService.Encrypt(prescriptionRequestId);
            return RedirectToAction(nameof(PescriptionRequest), new { medicationPescriptionId = encryptedId });
        }
    }
}
