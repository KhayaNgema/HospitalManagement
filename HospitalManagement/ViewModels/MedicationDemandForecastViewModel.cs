namespace HospitalManagement.ViewModels
{
    public class MedicationDemandForecastViewModel
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; }
        public int CurrentInventory { get; set; }
        public double AvgMonthlyUsage { get; set; }
        public double PredictedDemand { get; set; }
        public double StockCoverageMonths => AvgMonthlyUsage > 0 ? CurrentInventory / AvgMonthlyUsage : 0;
        public bool NeedsRestock { get; set; }
    }
}
