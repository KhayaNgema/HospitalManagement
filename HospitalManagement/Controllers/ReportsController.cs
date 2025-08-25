using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public async Task<IActionResult> Dashboards()
        {
            return View();
        }

        public async Task<IActionResult> Inventory()
        {
            int topMedicationsCount = 10;  // Adjust as needed

            // Load inventory data
            var inventoryData = await _context.MedicationInventory
                .Include(mi => mi.Medication)
                .ToListAsync();

            // Load usage data - aggregate total usage per medication
            var usageData = await _context.MedicationUsageLogs
                .GroupBy(u => new { u.MedicationId, u.Medication.MedicationName })
                .Select(g => new
                {
                    MedicationId = g.Key.MedicationId,
                    MedicationName = g.Key.MedicationName,
                    TotalUsed = g.Sum(u => u.QuantityDispensed)
                })
                .ToListAsync();

            var combinedData = inventoryData
     .Select(inv => new
     {
         MedicationName = inv.Medication.MedicationName,
         InventoryQuantity = inv.Quantity,
         UsageQuantity = usageData.FirstOrDefault(u => u.MedicationId == inv.MedicationId)?.TotalUsed ?? 0,
         StockLevel = inv.StockLevel
     })
     .Union(
         usageData
             .Where(u => !inventoryData.Any(inv => inv.MedicationId == u.MedicationId))
             .Select(u => new
             {
                 MedicationName = u.MedicationName,
                 InventoryQuantity = 0,
                 UsageQuantity = u.TotalUsed,
                 StockLevel = StockLevel.Critical  // Or choose appropriate fallback
             })
     )
     .OrderByDescending(d => d.InventoryQuantity + d.UsageQuantity)
     .Take(topMedicationsCount)
     .ToList();

            // Map stock levels to colors
            var stockLevelColors = combinedData.Select(d => d.StockLevel switch
            {
                StockLevel.High => "#28a745",       // Green
                StockLevel.Moderate => "#ffc107",   // Yellow
                StockLevel.Low => "#fd7e14",        // Orange
                StockLevel.Critical => "#dc3545",   // Red
                _ => "#6c757d"                      // Gray fallback
            }).ToArray();

            ViewBag.MedicationNames = combinedData.Select(d => d.MedicationName).ToArray();
            ViewBag.InventoryQuantities = combinedData.Select(d => d.InventoryQuantity).ToArray();
            ViewBag.UsageQuantities = combinedData.Select(d => d.UsageQuantity).ToArray();
            ViewBag.StockLevelColors = stockLevelColors;
            return View();
        }

        public async Task<IActionResult> MedicationUsage(DateTime? fromDate, DateTime? toDate)
        {
            DateTime today = DateTime.Today;
            DateTime startDate;
            DateTime endDate;

            if (fromDate.HasValue && toDate.HasValue && fromDate <= toDate)
            {
                startDate = fromDate.Value.Date;
                endDate = toDate.Value.Date;
            }
            else
            {
                // Default to last 12 months ending at current month
                endDate = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1); // end of current month
                startDate = endDate.AddMonths(-11).AddDays(1 - endDate.Day); // start of month 12 months ago
            }

            // Prepare months list for the line graph labels and data, spanning from startDate to endDate
            var monthsSpan = new List<DateTime>();
            var monthIterator = new DateTime(startDate.Year, startDate.Month, 1);
            var endMonth = new DateTime(endDate.Year, endDate.Month, 1);
            while (monthIterator <= endMonth)
            {
                monthsSpan.Add(monthIterator);
                monthIterator = monthIterator.AddMonths(1);
            }

            var usageLogs = await _context.MedicationUsageLogs
                .Include(x => x.Medication)
                .ThenInclude(x => x.Category)
                .Where(x => x.DispensedOn.Date >= startDate && x.DispensedOn.Date <= endDate)
                .ToListAsync();

            var usageData = usageLogs
                .GroupBy(x => new { x.DispensedOn.Year, x.DispensedOn.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalQuantity = g.Sum(x => x.QuantityDispensed)
                })
                .ToList();

            var lineChartData = monthsSpan
                .Select(m =>
                {
                    var dataItem = usageData.FirstOrDefault(u => u.Year == m.Year && u.Month == m.Month);
                    return new
                    {
                        MonthLabel = m.ToString("MMM yy"),
                        TotalQuantity = dataItem?.TotalQuantity ?? 0
                    };
                })
                .ToList();

            var categoryData = usageLogs
                .GroupBy(x => x.Medication.Category)
                .Select(g => new
                {
                    Category = g.Key.CategoryName,
                    TotalQuantity = g.Sum(x => x.QuantityDispensed)
                })
                .OrderByDescending(c => c.TotalQuantity)
                .ToList();

            var medicationsUsed = usageLogs
                .GroupBy(x => new { x.MedicationId, x.Medication.MedicationName })
                .Select(g => new
                {
                    Name = g.Key.MedicationName,
                    Quantity = g.Sum(x => x.QuantityDispensed)
                })
                .OrderByDescending(m => m.Quantity)
                .Take(6)
                .ToList();

            ViewBag.LineChartLabels = lineChartData.Select(x => x.MonthLabel).ToArray();
            ViewBag.LineChartValues = lineChartData.Select(x => x.TotalQuantity).ToArray();
            ViewBag.PieChartLabels = categoryData.Select(x => x.Category).ToArray();
            ViewBag.PieChartValues = categoryData.Select(x => x.TotalQuantity).ToArray();
            ViewBag.MedicationsUsed = medicationsUsed;
            ViewBag.FilterFromDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.FilterToDate = endDate.ToString("yyyy-MM-dd");

            return View();
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
