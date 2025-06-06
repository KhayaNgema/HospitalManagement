using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public BillingsController(HospitalManagementDbContext context,
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
        [Authorize]
        public async Task<IActionResult> MyBill()
        {
            var user = await _userManager.GetUserAsync(User);

            var patientBill = await _context.PatientBills
                .Where(pb => pb.PatientId == user.Id)
                .FirstOrDefaultAsync();

            var myBillServices = await _context.PatientBillServices
                .Include(mi => mi.PatientBill)
                .Include(mi => mi.Admission)
                .ThenInclude(mi => mi.Booking)
                .Include(mi => mi.Booking)
                .Where(mi => mi.PatientBillId == patientBill.BillId)
                .ToListAsync();

            ViewBag.TotalAmount = patientBill.PayableTotalAmount;

            return View(myBillServices);
        }
    }
}
