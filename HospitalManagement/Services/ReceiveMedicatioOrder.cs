using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HospitalManagement.Services
{
    public class ReceiveMedicationOrder
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        public readonly SmsService _smsService;

        public ReceiveMedicationOrder(HospitalManagementDbContext context,
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

        public async Task NotifyPharmacistOrderOnTheWayAsync(string encryptedOrderId)
        {
            int orderId = int.Parse(_encryptionService.Decrypt(encryptedOrderId));

            var order = await _context.MedicationOrders
                .Include(o => o.Pharmacist)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Pharmacist == null)
                return;

            var pharmacist = order.Pharmacist;
            var phone = pharmacist.PhoneNumber;
            var email = pharmacist.Email;

            string baseUrl = "https://hospitalmanagement2025group30-e4hfgeekephkc0fr.southafricanorth-01.azurewebsites.net";

            string encodedOrderId = WebUtility.UrlEncode(encryptedOrderId);

            string receiveOrderLink = $"{baseUrl}/Orders/ReceiveOrder?orderId={encodedOrderId}";

            string smsMessage = $"Dear Pharmacist {pharmacist.FirstName}, your order {order.OrderNumber} is now On The Way. " +
                                $"Please track or receive the order here: {receiveOrderLink}";

            string emailMessage = $@"Dear Pharmacist {pharmacist.FirstName}, your order {order.OrderNumber} is now On The Way. 
            Please <a href=""{receiveOrderLink}"">track or receive the order here</a>.";

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
                        subject: "Your Medication Order is On The Way",
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
