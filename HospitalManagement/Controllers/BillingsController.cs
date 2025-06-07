using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using HospitalManagement.Helpers;

namespace HospitalManagement.Controllers
{
    public class BillingsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly DeviceInfoService _deviceInfoService;
        private readonly FileUploadService _fileUploadService;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IViewRenderService _viewRenderService;

        public BillingsController(
            HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            FileUploadService fileUploadService,
            DeviceInfoService deviceInfoService,
            QrCodeService qrCodeService,
            ITempDataProvider tempDataProvider,
             IViewRenderService viewRenderService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
            _qrCodeService = qrCodeService;
            _deviceInfoService = deviceInfoService;
            _tempDataProvider = tempDataProvider;
            _viewRenderService = viewRenderService;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Transactions()
        {
            var user = await _userManager.GetUserAsync(User);

            var payments = await _context.Payments
                .Where(pb => pb.PaymentMadeById == user.Id && pb.Status == PaymentPaymentStatus.Successful)
                .OrderByDescending(pb => pb.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyBill()
        {
            var user = await _userManager.GetUserAsync(User);

            var patientBill = await _context.PatientBills
                .FirstOrDefaultAsync(pb => pb.PatientId == user.Id);

            var myBillServices = await _context.PatientBillServices
                .Include(mi => mi.PatientBill)
                .Include(mi => mi.Admission)
                    .ThenInclude(mi => mi.Booking)
                .Include(mi => mi.Booking)
                .Where(mi => mi.PatientBillId == patientBill.BillId)
                .ToListAsync();

            ViewBag.TotalAmount = patientBill?.PayableTotalAmount;

            return View(myBillServices);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PayBill()
        {
            try
            {
                var deviceInfo = await _deviceInfoService.GetDeviceInfo();
                var user = await _userManager.GetUserAsync(User);

                var patientBill = await _context.PatientBills
                    .FirstOrDefaultAsync(pb => pb.PatientId == user.Id);

                var newPayment = new Payment
                {
                    ReferenceNumber = GeneratePaymentReferenceNumber(),
                    PaymentMethod = PaymentMethod.Credit_Card,
                    AmountPaid = patientBill.PayableTotalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentMadeById = user.Id,
                    Status = PaymentPaymentStatus.Unsuccessful,
                    DeviceInfoId = deviceInfo.DeviceInfoId,
                };

                _context.Add(newPayment);
                await _context.SaveChangesAsync();

                var returnUrl = Url.Action("PayFastReturn", "Billings", new
                {
                    paymentId = newPayment.PaymentId,
                    amount = newPayment.AmountPaid
                }, Request.Scheme);

                returnUrl = HttpUtility.UrlEncode(returnUrl);
                var cancelUrl = "https://102.37.16.88:2002/Billings/MyBill";

                var paymentUrl = await GeneratePayFineFastPaymentUrl(newPayment.PaymentId, newPayment.AmountPaid, returnUrl, cancelUrl);

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to pay bill: " + ex.Message,
                    errorDetails = new
                    {
                        ex.InnerException?.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [Authorize]
        public async Task<IActionResult> PayFastReturn(int paymentId, decimal amount)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
                if (payment == null) return Json(new { success = false, message = "Payment not found." });

                var patientBill = await _context.PatientBills
                    .FirstOrDefaultAsync(pb => pb.PatientId == user.Id);

                var myBillServices = await _context.PatientBillServices
                    .Include(mi => mi.PatientBill)
                    .Include(mi => mi.Admission)
                        .ThenInclude(mi => mi.Booking)
                    .Include(mi => mi.Booking)
                    .Where(mi => mi.PatientBillId == patientBill.BillId)
                    .ToListAsync();

                var patient = patientBill.Patient;

                TempData["Message"] = $"You have successfully cleared your services bill amount of {patientBill.PayableTotalAmount}. Thank you for your payment.";

                patientBill.PayableTotalAmount = 0;
                payment.Status = PaymentPaymentStatus.Successful;

                _context.PatientBillServices.RemoveRange(myBillServices);
                _context.Update(patient);
                _context.Update(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyBill));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to process payment: " + ex.Message,
                    errorDetails = new
                    {
                        ex.InnerException?.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        private async Task<string> GeneratePayFineFastPaymentUrl(int paymentId, decimal amount, string returnUrl, string cancelUrl)
        {
            var user = await _userManager.GetUserAsync(User);

            var patientBill = await _context.PatientBills
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(pb => pb.PatientId == user.Id);

            if (patientBill == null || patientBill.Patient == null)
                throw new Exception("Bill or associated patient not found.");

            string merchantId = "10033052";
            string merchantKey = "708c7udni72oo";

            int amountInCents = (int)(amount * 100);
            string amountString = amount.ToString("0.00").Replace(',', '.');

            string billDetails = "MediConnect Services Bill";

            return $"https://sandbox.payfast.co.za/eng/process?" +
                   $"merchant_id={merchantId}&merchant_key={merchantKey}" +
                   $"&return_url={returnUrl}&cancel_url={cancelUrl}" +
                   $"&amount={amountInCents}&item_name={billDetails} of {patientBill.Patient.FirstName} {patientBill.Patient.LastName} bills." +
                   $"&payment_id={paymentId}&bill_id={patientBill.BillId}&amount={amountString}";
        }

        private string GeneratePaymentReferenceNumber()
        {
            var date = DateTime.Now;
            var random = new Random();
            string numbers = new string(Enumerable.Repeat("0123456789", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{date:yyMMdd}{numbers}BIL";
        }
    }
}
