using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HospitalManagement.ViewModels;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class AdmissionsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;

        public AdmissionsController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Admissions()
        {
            var admissions = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.CreatedBy)
                .Include(a => a.ModifiedBy)
                .ToListAsync(); 

            return View(admissions);
        }

        [HttpGet]
        public async Task<IActionResult> AdmissionDetails()
        {
            return View();
        }

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
                ProfilePicture = appointment.CreatedBy.ProfilePicture
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AdmitPatient(AdmitPatientViewModel viewModel)
        {

            try
            {
                var user = await _userManager.GetUserAsync(User);

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
                    PatientMedicalHistoryId= viewModel.PatientMedicalHistoryId,
                    PatientId= viewModel.PatientId,
                    PatientStatus = PatientStatus.Admitted,
                    RoomNumber = viewModel.RoomNumber,
                    Ward = viewModel.Ward,
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
                    $"to {viewModel.Department} in the {viewModel.Ward} at rom {viewModel.RoomNumber} " +
                    $"on bed {viewModel.BedNumber}.";

                return RedirectToAction(nameof(Admissions));
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

        [HttpGet]
        public async Task<IActionResult> UpdateAdmission()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdmission(UpdateAdmissionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

            }

            return View(viewModel);
        }
    }
}
