using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class StockMenuViewModel
    {
        public List<MedicationStock> MedicationStockItems { get; set; }
        public List<MedicationCartItem> MedicationCartItems { get; set; }
    }
}
