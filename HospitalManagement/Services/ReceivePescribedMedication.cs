using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HospitalManagement.Services
{
    public class ReceivePescribedMedication
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        public readonly SmsService _smsService;

        public ReceivePescribedMedication(HospitalManagementDbContext context,
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

        public async Task NotifyPatientOnTheWayAsync(string encryptedPackageId)
        {
            int packageId = int.Parse(_encryptionService.Decrypt(encryptedPackageId));

            var package = await _context.Packages
                .Include(o => o.DeliveryRequest)
                   .ThenInclude(o => o.Patient)
                .FirstOrDefaultAsync(o => o.PackageId == packageId);

            if (package == null || package.DeliveryRequest.Patient == null)
                return;

            var patient = package.DeliveryRequest.Patient;
            var phone = patient.PhoneNumber;
            var email = patient.Email;

            string baseUrl = "https://4.222.233.134:2005";

            string encodedPackageId = WebUtility.UrlEncode(encryptedPackageId);

            string receiveMedicationLink = $"{baseUrl}/Deliveries/ScanPackage?packageId={encodedPackageId}";

            string smsMessage = $"Dear {patient.FirstName}, your medication is now on the way. " +
                                $"Please track or receive the medication here: {receiveMedicationLink}";

            string emailMessage = $@"Dear {patient.FirstName}, your medication is now on the way. 
            Please <a href=""{receiveMedicationLink}"">track or receive the medication here</a>.";

            if (!string.IsNullOrEmpty(phone))
            {
                try
                {
                    await _smsService.SendSmsAsync(phone, smsMessage);
                }
                catch (Exception smsEx)
                {
                    Console.WriteLine($"Failed to send SMS to {phone}: {smsEx.Message}");
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        to: email,
                        subject: "Your Medication is On The Way",
                        body: emailMessage,
                        senderName: "Medi Care"
                    );
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Failed to send email to {email}: {emailEx.Message}");
                }
            }
        }
    }
}
