using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HospitalManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly DeviceInfoService _deviceInfoService;
        private readonly FileUploadService _fileUploadService;
        private readonly MedicationDemandService _demandService;

        public ReportsController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            FileUploadService fileUploadService,
            DeviceInfoService deviceInfoService,
            QrCodeService qrCodeService,
            MedicationDemandService demandService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
            _qrCodeService = qrCodeService;
            _deviceInfoService = deviceInfoService;
            _demandService = demandService;
        }

        public async Task<IActionResult> DemandDashboard(int medicationId)
        {
            var forecast = await _demandService.GetDemandForecastAsync(medicationId);
            if (forecast == null)
            {
                return NotFound();
            }

            var rawMonthlyUsage = await _demandService.GetMonthlyUsageAsync(medicationId);


            var today = DateTime.Today;
            var last12Months = Enumerable.Range(0, 12)
                .Select(i => today.AddMonths(-i))
                .Select(d => new { Year = d.Year, Month = d.Month })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var usageDict = rawMonthlyUsage.ToDictionary(
                u => $"{u.Year}-{u.Month}",
                u => u.TotalDispensed
            );

            var completedMonthlyUsage = last12Months
                .Select(m => new
                {
                    Year = m.Year,
                    Month = m.Month,
                    TotalDispensed = usageDict.TryGetValue($"{m.Year}-{m.Month}", out var val) ? val : 0
                })
                .ToList();

            ViewBag.MonthlyUsageJson = JsonSerializer.Serialize(completedMonthlyUsage);

            return View(forecast);
        }



        public async Task<IActionResult> MedicineReport()
        {
            return View();
        }

        public async Task<IActionResult> MedicineInventoryReport()
        {
            return View();
        }

        public async Task<IActionResult> AdmissionsReport()
        {
            return View();
        }

        public async Task<IActionResult> AppointmentsReport()
        {
            return View();
        }
    }
}
