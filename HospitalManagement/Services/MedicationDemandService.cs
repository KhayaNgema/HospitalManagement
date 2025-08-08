using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

public class MedicationDemandService
{
    private readonly HospitalManagementDbContext _context;

    public MedicationDemandService(HospitalManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<MonthlyMedicationUsage>> GetMonthlyUsageAsync(int medicationId)
    {
        var usageData = await _context.MedicationUsageLogs
            .Where(log => log.MedicationId == medicationId)
            .GroupBy(log => new { log.DispensedOn.Year, log.DispensedOn.Month })
            .Select(g => new MonthlyMedicationUsage
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalDispensed = g.Sum(x => x.QuantityDispensed)
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync();

        return usageData;
    }

    public double CalculateAverageMonthlyUsage(List<MonthlyMedicationUsage> usageData)
    {
        if (!usageData.Any()) return 0;
        return usageData.Average(x => x.TotalDispensed);
    }

    public double CalculateWeightedAverageUsage(List<MonthlyMedicationUsage> usageData)
    {
        if (!usageData.Any()) return 0;

        var weightedSum = usageData
            .Select((x, i) => new { Weight = i + 1, Value = x.TotalDispensed })
            .Sum(x => x.Weight * x.Value);

        var totalWeight = usageData.Select((_, i) => i + 1).Sum();

        return weightedSum / totalWeight;
    }

    public async Task<bool> ShouldRecommendRestockAsync(int medicationId, double predictedDemand)
    {
        var currentInventory = await _context.MedicationInventory
            .Where(i => i.MedicationId == medicationId)
            .Select(i => i.Quantity)
            .FirstOrDefaultAsync();


        return currentInventory < predictedDemand * 2;
    }

    public async Task<MedicationDemandForecastViewModel> GetDemandForecastAsync(int medicationId)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(m => m.MedicationId == medicationId);

        if (medication == null)
            return null;

        var usageData = await GetMonthlyUsageAsync(medicationId);
        var avgUsage = CalculateWeightedAverageUsage(usageData);

        var currentInventory = await _context.MedicationInventory
            .Where(i => i.MedicationId == medicationId)
            .Select(i => i.Quantity)
            .FirstOrDefaultAsync();

        var needsRestock = await ShouldRecommendRestockAsync(medicationId, avgUsage);

        return new MedicationDemandForecastViewModel
        {
            MedicationId = medicationId,
            MedicationName = medication.MedicationName,
            CurrentInventory = currentInventory,
            AvgMonthlyUsage = avgUsage,
            PredictedDemand = avgUsage,
            NeedsRestock = needsRestock
        };
    }
}