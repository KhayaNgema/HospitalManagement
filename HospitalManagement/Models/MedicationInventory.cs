using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationInventory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryId { get; set; }

        public int MedicationId { get; set; }
        public virtual Medication Medication { get; set; }

        public int Quantity { get; set; }

        public MedicationAvailability Availability { get; set; }

        public StockLevel StockLevel { get; set; }
    }

    public enum StockLevel
    {
        High,
        Moderate,
        Low,
        Critical
    }
}
