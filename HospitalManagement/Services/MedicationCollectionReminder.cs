using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class MedicationCollectionReminder
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        public readonly SmsService _smsService;

        public MedicationCollectionReminder(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            QrCodeService qrCodeService,
            EmailService emailService,
            SmsService smsService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _smsService = smsService;
        }

        public async Task SendRemindersAsync()
        {
            var today = DateTime.Today;
            var targetDate = today.AddDays(2);

            var prescriptions = await _context.MedicationPescription
                .Include(p => p.Admission)
                    .ThenInclude(a => a.Patient)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Patient)
                .Where(p => p.NextCollectionDate.HasValue &&
                            p.NextCollectionDate.Value.Date == targetDate.Date &&
                            p.HasDoneCollecting != true)
                .ToListAsync();

            foreach (var prescription in prescriptions)
            {
                var patient = prescription.Admission?.Patient ?? prescription.Booking?.Patient;
                if (patient == null) continue;

                bool reminderExists = await _context.MedicationReminders
                    .AnyAsync(r =>
                        r.MedicationPescriptionId == prescription.MedicationPescriptionId &&
                        r.Status == ReminderStatus.Sent &&
                        r.ExpiryDate.Date == (prescription.NextCollectionDate ?? DateTime.Now).Date);

                if (reminderExists)
                    continue;

                string link = "https://hospitalmanagement2025group30-e4hfgeekephkc0fr.southafricanorth-01.azurewebsites.net/Medications/Reminders";
                string message = $"Dear {patient.FirstName} {patient.LastName}, your medication will be ready for collection in 3 days on {prescription.NextCollectionDate:dd MMM yyyy}. " +
                                 $"Please ensure timely pickup.\nYou can indicate whether you'll collect it in person or prefer to have it delivered by visiting: {link}";

                if (!string.IsNullOrEmpty(patient.PhoneNumber))
                {
                    try
                    {
                        await _smsService.SendSmsAsync(patient.PhoneNumber, message);
                    }
                    catch (Exception smsEx)
                    {
                        Console.WriteLine($"Failed to send SMS to {patient.PhoneNumber}: {smsEx.Message}");
                    }
                }

                if (!string.IsNullOrEmpty(patient.Email))
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            to: patient.Email,
                            subject: "Medication Collection Reminder",
                            body: message,
                            senderName: "Medi Care"
                        );
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Failed to send email to {patient.Email}: {emailEx.Message}");
                    }
                }

                var reminder = new MedicationReminder
                {
                    MedicationPescriptionId = prescription.MedicationPescriptionId,
                    SentDate = DateTime.Now,
                    ExpiryDate = prescription.NextCollectionDate ?? DateTime.Now.AddDays(3),
                    Status = ReminderStatus.Sent,
                    ReminderMessage = message
                };

                _context.MedicationReminders.Add(reminder);
            }

            await _context.SaveChangesAsync();
        }
    }
}
