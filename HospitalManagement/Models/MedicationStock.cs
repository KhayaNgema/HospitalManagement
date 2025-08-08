using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationStock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockId { get; set; }

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }


        public int MedicationId { get; set; }
        public virtual Medication Medication { get; set; }

        public int StockCategoryId { get; set; }
        public virtual StockCategory StockCategory { get; set; }

        public int Quantity { get; set; }

        public string BatchNumber { get; set; }

        public byte[]? QrCodeImage { get; set; }
    }
}
