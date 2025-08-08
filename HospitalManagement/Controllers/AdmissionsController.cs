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
    public class AdmissionsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public AdmissionsController(HospitalManagementDbContext context,
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


        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyPatientAdmissions()
        {
            var user = await _userManager.GetUserAsync(User);

            var admissions = await _context.Admissions
                .Include(pa => pa.Patient)
                .Include(pa => pa.CreatedBy)
                .Where(pa => pa.CreatedById == user.Id)
                .OrderByDescending(pa => pa.CreatedAt)
                .ToListAsync();

            return View(admissions);
        }


        [Authorize]
        public async Task<IActionResult> MyAdmissions()
        {
            var user = await _userManager.GetUserAsync(User);

            var admissions = await _context.Admissions
                .Include(pa => pa.Patient)
                .Include(pa => pa.CreatedBy)
                .Where(pa => pa.PatientId == user.Id)
                .OrderByDescending(pa => pa.CreatedAt)
                .ToListAsync();

            return View(admissions);
        }


        [Authorize(Roles = "Doctor, System Administrator")]
        public async Task<IActionResult> Admissions()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            List<Admission> admissions;

            if (roles.Contains("System Administrator"))
            {

                admissions = await _context.Admissions
                    .Include(pa => pa.Patient)
                    .Include(pa => pa.CreatedBy)
                    .OrderByDescending(pa => pa.CreatedAt)
                    .ToListAsync();
            }
            else if (roles.Contains("Doctor"))
            {
                admissions = await _context.Admissions
                    .Include(pa => pa.Patient)
                    .Include(pa => pa.CreatedBy)
                    .Where(pa => pa.CreatedById == user.Id)
                    .OrderByDescending(pa => pa.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return View(admissions);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AdmissionDetails(string admissionId)
        {
            var decryptedAdmissionId = _encryptionService.DecryptToInt(admissionId);

            var admission = await _context.Admissions
                .Where(a => a.AdmissionId == decryptedAdmissionId)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync();

            var viewModel = new AdmissionDetailsViewModel
            {
                AdmissionId = decryptedAdmissionId,
                PatientId = admission.PatientId,
                BookingId = admission.BookingId,
                AdditionalNotes = admission.AdditionalNotes,
                Address = admission.Patient.Address,
                AdmissionDate = admission.AdmissionDate,
                AlternatePhoneNumber = admission.Patient.AlternatePhoneNumber,
                CollectAfterCount = 0,
                NextAppointmentDate = DateTime.Now,
                BedNumber = admission.BedNumber,
                DateOfBirth = admission.Patient.DateOfBirth,
                Department = admission.Department,
                DischargeDate = admission.DischargeDate,
                Email = admission.Patient.Email,
                FirstName = admission.Patient.FirstName,
                Gender = admission.Patient.Gender,
                IdNumber = admission.Patient.IdNumber,
                LastName = admission.Patient.LastName,
                LastVisitDate = admission.LastVisitDate,
                PatientMedicalHistoryId = admission.PatientMedicalHistoryId,
                PatientStatus = admission.PatientStatus,
                PhoneNumber = admission.Patient.PhoneNumber,
                ProfilePicture = admission.Patient.ProfilePicture,
                UntilDate = DateTime.Now,
                RoomNumber = admission.RoomNumber,
            };

            var medication = await _context.Medications
                .ToListAsync();

            ViewBag.Medications = medication;

            return View(viewModel);
        }


        [Authorize(Roles = "Doctor, System Administrator")]
        [HttpGet]
        public async Task<IActionResult> AdmitPatient(string appointmentId)
        {
            var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

            var appointment = await _context.Bookings
                .Where(a => a.BookingId == decryptedAppointmentId)
                .Include(a => a.CreatedBy)
                .FirstOrDefaultAsync();

            var patientMedicalHistory = await _context.PatientMedicalHistories
                .Where(pmh => pmh.PatientId == appointment.CreatedBy.Id)
                .OrderByDescending(pmh => pmh.CreatedAt)
                .FirstOrDefaultAsync();

            var rooms = await _context.Rooms
                .ToListAsync();

            var viewModel = new AdmitPatientViewModel
            {
                BookingId = decryptedAppointmentId,
                AdmissionDate = DateTime.Now,
                NextAppointmentDate = DateTime.Now,
                PatientMedicalHistoryId = patientMedicalHistory.PatientMedicalHistoryId,
                DischargeDate = DateTime.Now,
                PatientId = appointment.CreatedById,
                FirstName = appointment.CreatedBy.FirstName,
                LastName = appointment.CreatedBy.LastName,
                Address = appointment.CreatedBy.Address,
                AlternatePhoneNumber = appointment.CreatedBy.AlternatePhoneNumber,
                DateOfBirth = appointment.CreatedBy.DateOfBirth,
                Gender = appointment.CreatedBy.Gender,
                Email = appointment.CreatedBy.Email,
                IdNumber = appointment.CreatedBy.IdNumber,
                PhoneNumber = appointment.CreatedBy.PhoneNumber,
                ProfilePicture = appointment.CreatedBy.ProfilePicture,

            };

            ViewBag.Rooms = rooms;

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor, System Administrator")]
        [HttpPost]
        public async Task<IActionResult> AdmitPatient(AdmitPatientViewModel viewModel)
        {

            try
            {


                var user = await _userManager.GetUserAsync(User);

                var existingAdmission = await _context.Admissions
                    .Where(ae => ae.PatientId == viewModel.PatientId &&
                    ae.PatientStatus == PatientStatus.Admitted)
                    .FirstOrDefaultAsync();

                if (existingAdmission != null)
                {
                    TempData["Message"] = $"You cannot admit {viewModel.FirstName} {viewModel.LastName} " +
                        $"since they are currently admitted by {existingAdmission.Department}" +
                        $"at {existingAdmission.RoomNumber} on bed {existingAdmission.BedNumber}";

                    var rooms = await _context.Rooms
                        .ToListAsync();

                    ViewBag.Rooms = rooms;

                    return View(viewModel);
                }

                var admission = new Admission
                {
                    AdmissionDate = viewModel.AdmissionDate,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    BedNumber = viewModel.BedNumber,
                    NextAppointmentDate = viewModel.NextAppointmentDate,
                    Department = viewModel.Department,
                    DischargeDate = viewModel.DischargeDate,
                    CreatedById = user.Id,
                    LastVisitDate = DateTime.Now,
                    PatientMedicalHistoryId = viewModel.PatientMedicalHistoryId,
                    PatientId = viewModel.PatientId,
                    PatientStatus = PatientStatus.Admitted,
                    RoomNumber = viewModel.RoomNumber,
                    UpdatedById = user.Id,
                    BookingId = viewModel.BookingId,
                };

                _context.Add(admission);
                await _context.SaveChangesAsync();

                var appointment = await _context.Bookings
                    .Where(a => a.BookingId == viewModel.BookingId)
                    .FirstOrDefaultAsync();

                appointment.Status = BookingStatus.Completed;

                _context.Update(appointment);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully admitted {viewModel.FirstName} {viewModel.LastName} " +
                    $"to {viewModel.Department} department/ ward at room {viewModel.RoomNumber} " +
                    $"on bed {viewModel.BedNumber}.";

                return RedirectToAction(nameof(MyPatientAdmissions));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to admit patient: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }


            return View(viewModel);
        }

        [Authorize(Roles = "Doctor, System Administrator")]
        [HttpGet]
        public async Task<IActionResult> UpdateAdmission(string admissionId)
        {
            var decryptedAdmissionId = _encryptionService.DecryptToInt(admissionId);

            var admission = await _context.Admissions
                .Where(a => a.AdmissionId == decryptedAdmissionId)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync();

            var rooms = await _context.Rooms
                .ToListAsync();

            var viewModel = new UpdateAdmissionViewModel
            {
                AdmissionId = decryptedAdmissionId,
                PatientId = admission.PatientId,
                BookingId = admission.BookingId,
                AdditionalNotes = admission.AdditionalNotes,
                Address = admission.Patient.Address,
                AdmissionDate = admission.AdmissionDate,
                AlternatePhoneNumber = admission.Patient.AlternatePhoneNumber,
                CollectAfterCount = 0,
                NextAppointmentDate = DateTime.Now,
                BedNumber = admission.BedNumber,
                DateOfBirth = admission.Patient.DateOfBirth,
                Department = admission.Department,
                DischargeDate = admission.DischargeDate,
                Email = admission.Patient.Email,
                FirstName = admission.Patient.FirstName,
                Gender = admission.Patient.Gender,
                IdNumber = admission.Patient.IdNumber,
                LastName = admission.Patient.LastName,
                LastVisitDate = admission.LastVisitDate,
                PatientMedicalHistoryId = admission.PatientMedicalHistoryId,
                PatientStatus = admission.PatientStatus,
                PhoneNumber = admission.Patient.PhoneNumber,
                ProfilePicture = admission.Patient.ProfilePicture,
                UntilDate = DateTime.Now,
                RoomNumber = admission.RoomNumber,
            };

            var medication = await _context.Medications
                .ToListAsync();

            ViewBag.Medications = medication;

            ViewBag.Rooms = rooms;

            return View(viewModel);
        }


        [Authorize(Roles = "Doctor, System Administrator")]
        [HttpPost]
        public async Task<IActionResult> UpdateAdmission(UpdateAdmissionViewModel viewModel, string admissionId)
        {
            var decryptedAdmissionId = _encryptionService.DecryptToInt(admissionId);

            var admission = await _context.Admissions
                .Where(a => a.AdmissionId == decryptedAdmissionId)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync();


            admission.RoomNumber = viewModel.RoomNumber;
            admission.BedNumber = viewModel.BedNumber;
            admission.Department = viewModel.Department;
            admission.AdditionalNotes = viewModel.AdditionalNotes;
            admission.DischargeDate = viewModel.DischargeDate;

            _context.Update(admission);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"You have successfully updated {admission.Patient.FirstName}  {admission.Patient.LastName} admission";

            var encryptedAdmissionId = _encryptionService.Encrypt(admission.AdmissionId);

            return RedirectToAction(nameof(AdmissionDetails), new { admissionId = encryptedAdmissionId });

            var rooms = await _context.Rooms
                .ToListAsync();

            ViewBag.Rooms = rooms;

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor, System Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DischargePatient(AdmissionDetailsViewModel viewModel, string admissionId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var decryptedAdmissionId = _encryptionService.DecryptToInt(admissionId);

                var admission = await _context.Admissions
                    .Where(a => a.AdmissionId == decryptedAdmissionId)
                    .Include(a => a.Booking)
                    .ThenInclude(a => a.Patient)
                    .FirstOrDefaultAsync();

                admission.PatientStatus = PatientStatus.Discharged;
                admission.LastUpdatedAt = DateTime.Now;
                admission.AdditionalNotes = viewModel.AdditionalNotes;
                admission.UpdatedById = user.Id;

                _context.Update(admission);
                await _context.SaveChangesAsync();

                if (admission != null)
                {
                    var patientBill = await _context.PatientBills
                        .Where(pb => pb.PatientId == admission.PatientId)
                        .Include(pb => pb.Services)
                        .Include(pb => pb.Patient)
                        .FirstOrDefaultAsync();

                    if (viewModel.PrescribedMedication != null && viewModel.PrescribedMedication.Any())
                    {
                        DateTime baseDate = viewModel.LastCollectionDate ?? DateTime.Now;
                        int count = viewModel.CollectAfterCount ?? 0;

                        DateTime tentativeNextCollectionDate = baseDate;

                        switch (viewModel.CollectionInterval)
                        {
                            case CollectionInterval.Day:
                                tentativeNextCollectionDate = baseDate.AddDays(count);
                                break;
                            case CollectionInterval.Week:
                                tentativeNextCollectionDate = baseDate.AddDays(count * 7);
                                break;
                            case CollectionInterval.Month:
                                tentativeNextCollectionDate = baseDate.AddMonths(count);
                                break;
                            case CollectionInterval.Year:
                                tentativeNextCollectionDate = baseDate.AddYears(count);
                                break;
                        }

                        if (viewModel.UntilDate.HasValue && tentativeNextCollectionDate > viewModel.UntilDate.Value)
                        {
                            tentativeNextCollectionDate = viewModel.UntilDate.Value;
                        }


                        DateTime finalNextCollectionDate = (viewModel.UntilDate.HasValue && tentativeNextCollectionDate > viewModel.UntilDate.Value)
                            ? viewModel.UntilDate.Value
                            : tentativeNextCollectionDate;

                        var medicationPescription = new MedicationPescription
                        {
                            AdditionalNotes = viewModel.AdditionalNotes,
                            AdmissionId = decryptedAdmissionId,
                            BookingId = admission.BookingId,
                            CollectAfterCount = viewModel.CollectAfterCount,
                            CreatedAt = DateTime.Now,
                            LastUpdatedAt = DateTime.Now,
                            CollectionInterval = viewModel.CollectionInterval,
                            CreatedById = user.Id,
                            HasDoneCollecting = false,
                            NextCollectionDate = finalNextCollectionDate,
                            PrescribedMedication = new List<Medication>(),
                            PrescriptionType = viewModel.PrescriptionType,
                            UpdatedById = user.Id,
                            ExpiresAt = viewModel.UntilDate,
                            AccessCode = admission.Booking.BookingReference,
                            LastCollectionDate = baseDate,
                        };

                        _context.Add(medicationPescription);
                        await _context.SaveChangesAsync();

                        medicationPescription.QrCodeImage = _qrCodeService.GenerateQrCode(medicationPescription.AccessCode);
                        _context.Update(medicationPescription);
                        await _context.SaveChangesAsync();

                        foreach (var med in viewModel.PrescribedMedication)
                        {
                            var medicationEntity = await _context.Medications
                                .FirstOrDefaultAsync(m => m.MedicationId == med.MedicationId);

                            if (medicationEntity != null)
                            {
                                medicationPescription.PrescribedMedication.Add(medicationEntity);
                            }

                            if (medicationEntity != null && admission != null)
                            {
                                var newBillService = new PatientBillServices
                                {
                                    AdmissionId = admission.AdmissionId,
                                    BookingId = admission.BookingId,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    PatientBillId = patientBill.BillId,
                                    ReferenceNumber = "@HO-WARD-TRT",
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

                TempData["Message"] = $"You have successfully discharged {admission.Patient.FirstName} {admission.Patient.LastName}";


                return RedirectToAction(nameof(MyPatientAdmissions));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to discharge patient: " + ex.Message,
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
