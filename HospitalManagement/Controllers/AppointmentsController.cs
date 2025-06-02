using Hangfire;
using Hangfire.Dashboard;
using HospitalManagement.Data;
using HospitalManagement.Helpers;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Web;

namespace HospitalManagement.Controllers
{
    public class AppointmentsController : Controller
    {

        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly DeviceInfoService _deviceInfoService;
        private readonly FileUploadService _fileUploadService;

        public AppointmentsController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            FileUploadService fileUploadService,
            DeviceInfoService deviceInfoService,
            QrCodeService qrCodeService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
            _qrCodeService = qrCodeService;
            _deviceInfoService = deviceInfoService;

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
                var allAppointments = await query
                    .Where(ap => ap.Status == BookingStatus.Awaiting||
                    ap.Status == BookingStatus.Assigned)
                    .OrderBy(ap => ap.BookForDate)
                    .ThenBy(ap => ap.BookForTime)
                    .ToListAsync();

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

                var filteredAppointments = await query
                    .Where(a => allowedConditions.Contains(a.MedicalCondition) &&
                    a.Status == BookingStatus.Awaiting ||
                    a.Status == BookingStatus.Assigned)
                    .OrderBy(a => a.BookForDate)
                    .ThenBy(a => a.BookForTime)
                    .ToListAsync();

                return View(filteredAppointments);
            }

            return Forbid();
        }

        [HttpGet]
        public async Task<IActionResult> X_RayAppointments()
        {
            var appointments = await _context.X_RayAppointments
                .Include(a => a.CreatedBy)
                .Include(a => a.Doctor)
                .Include(a => a.ModifiedBy)
                .Include(a => a.Booking)
                .ToListAsync();

            return View(appointments);
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
        public async Task<IActionResult> XRayAppointmentDetails(string appointmentId)
        {
            var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

            var appointment = await _context.X_RayAppointments
                .Where(a => a.BookingId == decryptedAppointmentId)
                .Include(a => a.CreatedBy)
                .Include(a => a.ModifiedBy)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
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

            var viewModel = new XRayAppointmentDetailsViewModel
            {
                PatientId = appointment.CreatedById,
                DoctorId = appointment.CreatedById,
                BookingId = appointment.BookingId,
                CreatedAt = appointment.CreatedAt,
                LastUpdatedAt = appointment.LastUpdatedAt,
                BookForTime = appointment.BookForTime,
                BookForDate = appointment.BookForDate,
                Status = appointment.Status,
                AdditionalNotes = appointment.AdditionalNotes,
                BookingReference = appointment.BookingReference,
                DoctorFullNames = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                PatientFullNames = $"{appointment.CreatedBy.FirstName} {appointment.CreatedBy.LastName}",
                Instructions = appointment.Instructions,
                MedicalCondition = condition,
                OriginalBookingId = appointment.OriginalBookingId,
                PatientEmail = appointment.CreatedBy.Email,
                PatientProfilePicture = appointment.CreatedBy.ProfilePicture,
                PhoneNumber = appointment.CreatedBy.PhoneNumber,
                Specialization = specializations,
                XRayImage = appointment.ScannerImage
            };

            return View(viewModel);
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
                var deviceInfo = await _deviceInfoService.GetDeviceInfo();

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

                var patientMedicalRecord = await _context.PatientMedicalHistories
                    .Where(pmr => pmr.PatientId == user.Id)
                    .FirstOrDefaultAsync();

                if (patientMedicalRecord != null)
                {
                    patientMedicalRecord.AccessCode = newAppointment.BookingReference;
                    patientMedicalRecord.QrCodeImage = newAppointment.QrCodeImage;
                    _context.Update(patientMedicalRecord);
                    await _context.SaveChangesAsync();
                }

                var newPayment = new Payment
                {
                    ReferenceNumber = GeneratePaymentReferenceNumber(),
                    PaymentMethod = PaymentMethod.Credit_Card,
                    AmountPaid = 90,
                    PaymentDate = DateTime.Now,
                    PaymentMadeById = user.Id,
                    Status = PaymentPaymentStatus.Unsuccessful,
                    DeviceInfoId = deviceInfo.DeviceInfoId,
                };

                _context.Add(newPayment);
                await _context.SaveChangesAsync();

                var appointment = await _context.Bookings
                    .Where(a => a.BookingId == newAppointment.BookingId)
                    .Include(a => a.CreatedBy)
                    .FirstOrDefaultAsync();

                var payment = await _context.Payments
                    .Where(a => a.PaymentId == newPayment.PaymentId)
                    .FirstOrDefaultAsync();

                var encryptedAppointmentId = _encryptionService.Encrypt(appointment.BookingId);
                int paymentId = payment.PaymentId;
                decimal amount = newPayment.AmountPaid;
                string appointmentId = encryptedAppointmentId;

                var returnUrl = Url.Action("PayFastReturn", "Appointments", new { paymentId, appointmentId, amount }, Request.Scheme);
                returnUrl = HttpUtility.UrlEncode(returnUrl);
                var cancelUrl = "https://102.37.16.88:2002/";

                string paymentUrl = await GeneratePayFineFastPaymentUrl(paymentId, amount, appointmentId, returnUrl, cancelUrl);

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to make appointment: " + ex.Message,
                    errorDetails = new
                    {
                        innerException = ex.InnerException?.Message,
                        stackTrace = ex.StackTrace
                    }
                });
            }
        }


        public async Task<IActionResult> PayFastReturn(int paymentId, string appointmentId, decimal amount)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Payment with PaymentId: {paymentId} not found.");
                    return Json(new { success = false, message = "Payment not found." });
                }

                var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

                var appointment = await _context.Bookings
                    .Where(a => a.BookingId == decryptedAppointmentId)
                    .Include(a => a.CreatedBy)
                    .FirstOrDefaultAsync();

                var patient = appointment.CreatedBy;

                appointment.Status = BookingStatus.Awaiting;

                _context.Update(appointment);
                await _context.SaveChangesAsync();

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
                    message = "Failed to process payment: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }




        private async Task<string> GeneratePayFineFastPaymentUrl(int paymentId, decimal amount, string appointmentId, string returnUrl, string cancelUrl)
        {
            var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

            var appointment = await _context.Bookings
                .Where(a => a.BookingId == decryptedAppointmentId)
                .Include(a => a.CreatedBy)
                .FirstOrDefaultAsync();

            if (appointment == null || appointment.CreatedBy == null)
                throw new Exception("Appointment or associated patient not found.");


            var patient = appointment.CreatedBy;

            string merchantId = "10033052";
            string merchantKey = "708c7udni72oo";

            int amountInCents = (int)(amount * 100);
            string amountString = amount.ToString("0.00").Replace(',', '.');

            string appointmentDetails = "MediConnect Appointment Fee";

            string paymentUrl = $"https://sandbox.payfast.co.za/eng/process?" +
                                $"merchant_id={merchantId}&merchant_key={merchantKey}" +
                                $"&return_url={returnUrl}&cancel_url={cancelUrl}" +
                                $"&amount={amountInCents}&item_name={appointmentDetails} of {patient.FirstName} {patient.LastName} appointment for:{appointment.MedicalCondition}" +
                                $"&payment_id={paymentId}&fine_id={appointmentId}&amount={amountString}";

            return paymentUrl;
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusRedirect(int appointmentId, BookingStatus status, IFormFile XRayImages)
        {
            var user = await _userManager.GetUserAsync(User);
            var xrayAppointment = await _context.Bookings
                .OfType<X_RayAppointment>()
                .Include(b => b.Patient)
                .Include(b => b.ModifiedBy)
                .FirstOrDefaultAsync(b => b.BookingId == appointmentId);

            if (xrayAppointment != null)
            {
                xrayAppointment.Status = status;
                xrayAppointment.LastUpdatedAt = DateTime.Now;
                xrayAppointment.UpdatedById = user.Id;

                if (XRayImages != null && XRayImages.Length > 0)
                {
                    var playerProfilePicturePath = await _fileUploadService.UploadFileAsync(XRayImages);
                    xrayAppointment.ScannerImage = playerProfilePicturePath;
                }

                _context.Update(xrayAppointment);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully updated this appointment to {status}.";

                var encryptedId = _encryptionService.Encrypt(appointmentId);
                return RedirectToAction(nameof(XRayAppointmentDetails), new { appointmentId = encryptedId });
            }

            var booking = await _context.Bookings
                .Where(b => !(b is X_RayAppointment))
                .Include(b => b.Patient)
                .Include(b => b.ModifiedBy)
                .FirstOrDefaultAsync(b => b.BookingId == appointmentId);

            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;
            booking.LastUpdatedAt = DateTime.Now;
            booking.UpdatedById = user.Id;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"You have successfully updated this appointment to {status}.";

            var encryptedBookingId = _encryptionService.Encrypt(appointmentId);
            return RedirectToAction(nameof(AppointmentDetails), new { appointmentId = encryptedBookingId });
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


        private string GeneratePaymentReferenceNumber()
        {
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString("D2");
            var day = DateTime.Now.Day.ToString("D2");

            const string numbers = "0123456789";
            const string fineLetters = "BO";

            var random = new Random();
            var randomNumbers = new string(Enumerable.Repeat(numbers, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{year}{month}{day}{randomNumbers}{fineLetters}";
        }


        [HttpGet]
        public async Task<IActionResult> X_RayAppointment(string appointmentId)
        {
            var decryptedAppointmentId = _encryptionService.DecryptToInt(appointmentId);

            var appointment = await _context.Bookings
                .Where(a => a.BookingId == decryptedAppointmentId)
                .Include(a => a.CreatedBy)
                .Include(a => a.ModifiedBy)
                .FirstOrDefaultAsync();

            var patient = appointment.CreatedBy;

            var viewModel = new BookXRayViewModel
            {
                PatientId = patient.Id,
                AdditionalNotes = appointment.AdditionalNotes,
                BookForDate = DateTime.Now,
                BookForTime = DateTime.Now,
                MedicalCondition = appointment.MedicalCondition,
                BookingId = decryptedAppointmentId,
                Address = patient.Address,
                AlternatePhoneNumber= patient.AlternatePhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Email = patient.Email,
                FirstName = patient.FirstName,
                Gender = patient.Gender,
                IdNumber = patient.IdNumber,
                LastName = patient.LastName,
                PhoneNumber = patient.PhoneNumber,
                ProfilePicture = patient.ProfilePicture,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> X_RayAppointment(BookXRayViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var appointment = await _context.Bookings
                    .Where(a => a.BookingId == viewModel.BookingId)
                    .Include(a => a.CreatedBy)
                    .Include(a => a.ModifiedBy)
                    .FirstOrDefaultAsync();

                ICollection<string> instructionsList = null;

                if (!string.IsNullOrWhiteSpace(viewModel.InstructionsInput))
                {
                    instructionsList = viewModel.InstructionsInput
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(i => i.Trim())
                        .ToList();
                }

                var newXRayAppointment = new X_RayAppointment
                {
                    PatientId = viewModel.PatientId,
                    BodyParts = viewModel.BodyParts,
                    BookForDate = viewModel.BookForDate,
                    BookForTime = viewModel.BookForTime,
                    BookingReference = GenerateBookingReferenceNumber(),
                    AdditionalNotes = viewModel.AdditionalNotes,
                    CreatedAt = DateTime.Now,
                    CreatedById = appointment.CreatedById,
                    DoctorId = user.Id,
                    OriginalBookingId = viewModel.BookingId,
                    LastUpdatedAt = DateTime.Now,
                    Instructions = instructionsList, 
                    MedicalCondition = viewModel.MedicalCondition,
                    Status = BookingStatus.Awaiting,
                    ScannerImage = viewModel.ScannerImage,
                    UpdatedById = user.Id
                };

                _context.Add(newXRayAppointment);
                await _context.SaveChangesAsync();

                newXRayAppointment.QrCodeImage = _qrCodeService.GenerateQrCode(newXRayAppointment.BookingReference);
                _context.Update(newXRayAppointment);
                await _context.SaveChangesAsync();

                var patient = await _context.Patients
                    .Where(p => p.Id == viewModel.PatientId)
                    .FirstOrDefaultAsync();

                TempData["Message"] = $"You have successfully placed booked an X-Ray appointement " +
                    $"for {patient.FirstName} {patient.LastName} " +
                    $"for {viewModel.BookForDate.ToString("dd/MM/yyy")} " +
                    $"at {viewModel.BookForTime.ToString("HH:mm")}.";

                var encryptedAppointmentId = _encryptionService.Encrypt(viewModel.BookingId);

                return RedirectToAction(nameof(AppointmentDetails), new { appointmentId = encryptedAppointmentId });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to book XRay: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View();
        }



    }
}
