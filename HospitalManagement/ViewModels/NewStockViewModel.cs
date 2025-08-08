using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class NewStockViewModel
    {
        public int SupplierId { get; set; }

        public int MedicationId { get; set; }

        public int StockCategoryId { get; set; }

        public int Quantity { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }

        public Strength Strength { get; set; }

        public DosageForm DosageForm { get; set; }

        public string BatchNumber { get; set; }

        public string BarCode { get; set; }
    }
}
