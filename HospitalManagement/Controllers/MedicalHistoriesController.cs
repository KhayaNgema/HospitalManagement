using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class MedicalHistoriesController : Controller
    {

        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public MedicalHistoriesController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            QrCodeService qrCodeService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;

        }

        [HttpGet]
        public async Task<IActionResult> PatientsMedicalHistory()
        {
            var medicalHistory = await _context.PatientMedicalHistories
                .Include(x => x.Patient)
                .ToListAsync();

            return View(medicalHistory);
        }


        [HttpGet]
        public async Task<IActionResult> PatientMedicalRecord(string medicalHistoryId)
        {
            var decryptedMedicalHistoryId = _encryptionService.DecryptToInt(medicalHistoryId);

            var medicalRecord = await _context.PatientMedicalHistories
                .Where(mr => mr.PatientMedicalHistoryId == decryptedMedicalHistoryId)
                .Include(x => x.Patient) 
                .FirstOrDefaultAsync();

            if (medicalRecord != null)
            {
                await _context.Entry(medicalRecord)
                    .Collection(mr => mr.MedicalHistories)
                    .Query()
                    .Include(mh => mh.Doctor)
                    .OrderByDescending(mh => mh.CreatedAt)
                    .LoadAsync();
            }

            return View(medicalRecord);
        }



        [HttpGet]
        public async Task<IActionResult> NewMedicalRecord(string medicalHistoryId)
        {
            var decryptedMedicalHistoryId = _encryptionService.DecryptToInt(medicalHistoryId);

            var medicalHistory = await _context.PatientMedicalHistories
                .Where(p => p.PatientMedicalHistoryId == decryptedMedicalHistoryId)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync();

            var patient = medicalHistory.Patient;

            var viewModel = new NewMedicalRecordViewModel
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                ProfilePicture = patient.ProfilePicture,
                PatientId = medicalHistory.PatientId,
                PatientMedicalHistoryId= decryptedMedicalHistoryId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> NewMedicalRecord(NewMedicalRecordViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var newMedicalRecord = new MedicalHistory
                {
                    PatientId = viewModel.PatientId,
                    ChiefComplaint = viewModel.ChiefComplaint,
                    Diagnosis = viewModel.Diagnosis,
                    DoctorId = user.Id,
                    FollowUpInstructions = viewModel.FollowUpInstructions,
                    Immunizations = viewModel.Immunizations,
                    HeightCm = viewModel.HeightCm,
                    LabResults = viewModel.LabResults,
                    Medications = viewModel.Medications,
                    Notes = viewModel.Notes,
                    PatientMedicalHistoryId = viewModel.PatientMedicalHistoryId,
                    Surgeries = viewModel.Surgeries,
                    WeightKg = viewModel.WeightKg,
                    Vitals = viewModel.Vitals,
                    VisitDate = DateTime.Now,
                    Treatment = viewModel.Treatment,
                    Symptoms = viewModel.Symptoms,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    RecordedAt = DateTime.Now,
                    CreatedById = user.Id,
                    UpdatedById = user.Id,
                };

                _context.Add(newMedicalRecord);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added new medical record for {viewModel.FirstName} {viewModel.LastName}";

                var encryptedMedicalRecordId = _encryptionService.Encrypt(viewModel.PatientMedicalHistoryId);

                return RedirectToAction(nameof(PatientMedicalRecord), new { medicalHistoryId = encryptedMedicalRecordId });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add new medical record: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View(viewModel);
        }


    }
}
