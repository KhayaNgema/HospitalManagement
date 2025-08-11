using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> PatientsMedicalHistory()
        {
            var medicalHistory = await _context.PatientMedicalHistories
                .Include(x => x.Patient)
                .ToListAsync();

            return View(medicalHistory);
        }

        [Authorize(Roles = "Doctor")]
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

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> MedicalHistory(string medicalHistoryId)
        {
            var decryptedMedicalHistoryId = _encryptionService.DecryptToInt(medicalHistoryId);

            var medicalHistory = await _context.MedicalHistorys
                .Where(mh => mh.MedicalHistoryId == decryptedMedicalHistoryId)
                .Include(mh => mh.Patient)
                .FirstOrDefaultAsync();

            var viewModel = new MedicalHistoryViewModel
            {
                CollectAfterCount = medicalHistory.CollectAfterCount,
                ChiefComplaint = medicalHistory.ChiefComplaint,
                CollectionInterval = medicalHistory.CollectionInterval,
                FirstName = medicalHistory.Patient.FirstName,
                DateOfBirth = medicalHistory.Patient.DateOfBirth,
                Diagnosis = medicalHistory.Diagnosis,
                FollowUpInstructions = medicalHistory.FollowUpInstructions,
                HeightCm = medicalHistory.HeightCm,
                Immunizations = medicalHistory.Immunizations,
                LabResults = medicalHistory.LabResults,
                LastName = medicalHistory.Patient?.LastName,
                PrescribedMedication = medicalHistory.PrescribedMedication,
                PrescriptionType = medicalHistory.PrescriptionType,
                ProfilePicture = medicalHistory.Patient.ProfilePicture,
                Notes = medicalHistory.Notes,
                Surgeries = medicalHistory.Surgeries,
                Symptoms = medicalHistory.Symptoms,
                Treatment = medicalHistory.Treatment,
                UntilDate = medicalHistory.UntilDate,
                Vitals = medicalHistory.Vitals,
                WeightKg = medicalHistory.WeightKg
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> NewMedicalRecord(string medicalHistoryId)
        {
            var decryptedMedicalHistoryId = _encryptionService.DecryptToInt(medicalHistoryId);

            var medicalHistory = await _context.PatientMedicalHistories
                .Where(p => p.PatientMedicalHistoryId == decryptedMedicalHistoryId)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync();

            var appointment = await _context.Bookings
                .Where(a => a.BookingReference == medicalHistory.AccessCode)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            var patient = medicalHistory.Patient;

            var viewModel = new NewMedicalRecordViewModel
            {
                BookingId = appointment.BookingId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                ProfilePicture = patient.ProfilePicture,
                PatientId = medicalHistory.PatientId,
                PatientMedicalHistoryId = decryptedMedicalHistoryId
            };

            var medication = await _context.Medications
                .ToListAsync();

            var admission = await _context.Admissions
                .Where(m => m.PatientId == patient.Id &&
                m.PatientStatus == PatientStatus.Admitted)
                .FirstOrDefaultAsync();

            ViewBag.Medications = medication;

            ViewBag.Admission = admission;


            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
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
                    PrescribedMedication = null,
                    CollectAfterCount = viewModel.CollectAfterCount,
                    CollectionInterval = viewModel.CollectionInterval,
                    UntilDate = DateTime.Now,
                    PrescriptionType = viewModel.PrescriptionType
                };

                _context.MedicalHistorys.Add(newMedicalRecord);
                await _context.SaveChangesAsync();

                var admission = await _context.Admissions
                    .Where(a => a.PatientId == viewModel.PatientId &&
                    a.PatientStatus == PatientStatus.Admitted)
                    .FirstOrDefaultAsync();

                var booking = await _context.Bookings
                    .Where(b => b.BookingId == viewModel.BookingId)
                    .FirstOrDefaultAsync();

                if (booking != null)
                {
                    var patientBill = await _context.PatientBills
                        .Where(pb => pb.PatientId == viewModel.PatientId)
                        .Include(pb => pb.Services)
                        .FirstOrDefaultAsync();

                    if (viewModel.PrescribedMedication != null && viewModel.PrescribedMedication.Any())
                    {
                        foreach (var med in viewModel.PrescribedMedication)
                        {
                            var medicationEntity = await _context.Medications
                                .FirstOrDefaultAsync(m => m.MedicationId == med.MedicationId);

                            if(medicationEntity != null && booking != null)
                            {
                                var newBillService = new PatientBillServices
                                {
                                    AdmissionId = null,
                                    BookingId = booking.BookingId,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    PatientBillId = patientBill.BillId,
                                    ReferenceNumber = "@HO-1VST-TRT",
                                    ServiceName = medicationEntity.MedicationName ?? "Prescribed Medication",
                                    ServiceType = "Medication",
                                    Subtotal = medicationEntity.Price,
                                };

                                _context.Add(newBillService);

                                patientBill.Services.Add(newBillService);
                                patientBill.PayableTotalAmount += newBillService.Subtotal;
                                _context.Update(patientBill);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

                if(booking != null)
                {
                    var medicationPescription = new MedicationPescription
                    {
                        AdditionalNotes = viewModel.Notes,
                        AdmissionId = null,
                        CollectAfterCount = viewModel.CollectAfterCount,
                        CreatedAt = DateTime.Now,
                        LastUpdatedAt = DateTime.Now,
                        CollectionInterval = viewModel.CollectionInterval,
                        CreatedById = user.Id,
                        HasDoneCollecting = false,
                        ExpiresAt = viewModel.UntilDate,
                        NextCollectionDate = null,
                        PrescriptionType = null,
                        UpdatedById = user.Id,
                        BookingId = viewModel.BookingId,
                        PrescribedMedication = new List<Medication>(),
                        AccessCode = booking.BookingReference,

                    };

                    if (viewModel.PrescribedMedication != null && viewModel.PrescribedMedication.Any())
                    {
                        foreach (var med in viewModel.PrescribedMedication)
                        {
                            var medicationEntity = await _context.Medications
                                .FirstOrDefaultAsync(m => m.MedicationId == med.MedicationId);

                            if (medicationEntity != null)
                            {
                                medicationPescription.PrescribedMedication.Add(medicationEntity);
                            }
                            else
                            {

                            }
                        }
                    }

                    _context.Add(medicationPescription);
                    await _context.SaveChangesAsync();

                    medicationPescription.QrCodeImage = _qrCodeService.GenerateQrCode(medicationPescription.AccessCode);
                    _context.Update(medicationPescription);
                    await _context.SaveChangesAsync();
                }


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
        }


    }
}
