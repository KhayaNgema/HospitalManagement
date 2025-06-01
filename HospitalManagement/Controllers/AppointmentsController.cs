using Hangfire;
using HospitalManagement.Data;
using HospitalManagement.Helpers;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class AppointmentsController : Controller
    {

        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public AppointmentsController(HospitalManagementDbContext context,
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
        public async Task<IActionResult> Appointments()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(user);

            IQueryable<Booking> query = _context.Bookings
                .Include(a => a.Patient)
                .Include(a => a.CreatedBy)
                .Include(a => a.ModifiedBy);

            if (userRoles.Contains("System Administrator"))
            {
                var allAppointments = await query.Where(ap => ap.Status == BookingStatus.Pending).ToListAsync();

                return View(allAppointments);
            }
            else if (userRoles.Contains("Doctor"))
            {

                var doctor = await _context.Doctors
                    .Where(d => d.Id == user.Id)
                    .FirstOrDefaultAsync();

                var doctorSpecialization = doctor.Specialization; 


                var allowedConditions = ConditionToSpecializationsMap.Map
                    .Where(entry => entry.Value.Contains(doctorSpecialization))
                    .Select(entry => entry.Key)
                    .ToList();

                query = query.Where(a => allowedConditions.Contains(a.MedicalCondition));

                var filteredAppointments = await query.ToListAsync();
                return View(filteredAppointments);
            }

            return Forbid();
        }


        [HttpGet]
        public async Task<IActionResult> MyTeamAppointments()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);

            var myAppointments = await _context.Bookings
                .Where(ma => ma.CreatedById == user.Id)
                .OrderByDescending(ma => ma.CreatedAt)
                .ToListAsync(); 
                
            return View(myAppointments);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentDetails(string appointmentId)
        {
            var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

            var appointment = await _context.Bookings
                .Where(a => a.BookingId == decryptedAppointmentId)
                .Include(a => a.CreatedBy)
                .Include(a => a.ModifiedBy)
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            var condition = appointment.MedicalCondition;

            if (ConditionToSpecializationsMap.Map.TryGetValue(condition, out var specializations))
            {
                ViewBag.AssignedTeam = specializations;
            }
            else
            {
                ViewBag.AssignedTeam = new List<Specialization>();
            }

            return View(appointment);
        }



        [HttpGet]
        public async Task<IActionResult> MakeAppointment()
        {
            var user = await _userManager.GetUserAsync(User);

            var viewModel = new MakeAppointmentViewModel
            {
                PatientId = user.Id,
                BookForDate = DateTime.Today,
                BookForTime = DateTime.Today.Add(DateTime.Now.TimeOfDay)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAppointment(MakeAppointmentViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var bookingReference = GenerateBookingReferenceNumber();

                var newAppointment = new Booking
                {
                    PatientId = viewModel.PatientId,
                    BookForDate = viewModel.BookForDate,
                    BookForTime = viewModel.BookForTime,
                    MedicalCondition = viewModel.MedicalCondition,
                    AdditionalNotes = viewModel.AdditionalNotes,
                    Status = BookingStatus.Pending,
                    CreatedById = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    BookingReference = bookingReference,
                };

                _context.Add(newAppointment);
                await _context.SaveChangesAsync();

                newAppointment.QrCodeImage = _qrCodeService.GenerateQrCode(bookingReference);

                _context.Update(newAppointment);
                await _context.SaveChangesAsync();

                var appointment = await _context.Bookings
                    .Where(a => a.Equals(newAppointment))
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync();
                /*
                                string emailBody = $@"
                  <p>Dear {appointment.Patient.FirstName ?? ""} {appointment.Patient.LastName ?? ""},</p>

                    <p>You have successfully booked your appointment.</p>

                    <p><strong>Appointment Details:</strong></p>
                    <ul>
                        <li><strong>Date:</strong> {appointment.BookForDate:MMMM dd, yyyy}</li>
                        <li><strong>Time:</strong> {appointment.BookForTime:hh:mm tt}</li>
                        <li><strong>Medical Condition:</strong> {appointment.MedicalCondition}</li>
                        <li><strong>Status:</strong> {appointment.Status}</li>
                    </ul>

                    <p>If you have any questions, feel free to contact us.</p>

                    <p>Best regards,<br/>MediConnect Team</p>
                ";



                                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(appointment.Patient.Email, "Appointment Confirmation", emailBody, "MediConnect"));*/
                TempData["Message"] = $"You have successfully booked an appointment for {appointment.MedicalCondition} on {appointment.BookForDate:MMMM dd, yyyy} at {appointment.BookForTime:hh\\:mm tt}. " +
                                      $"We have sent you an email with your booking details. " +
                                      $"<a href='/Appointments/MyAppointments' style='color: inherit; text-decoration: underline;'>See Appointment</a>";

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex) 
            {

                return Json(new
                {
                    success = false,
                    message = "Failed to make appointment: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View(viewModel);
        }

        private string GenerateBookingReferenceNumber()
        {
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString("D2");
            var day = DateTime.Now.Day.ToString("D2");

            const string numbers = "0123456789";
            const string bookingLetters = "B";

            var random = new Random();
            var randomNumbers = new string(Enumerable.Repeat(numbers, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            var bookLetters = bookingLetters.ToString();

            return $"{year}{month}{day}{randomNumbers}{bookLetters}";
        }
    }
}
