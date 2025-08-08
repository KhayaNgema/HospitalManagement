using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationCartItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        public string PharmacistId { get; set; }
        [ForeignKey("PharmacistId")]
        public virtual Pharmacist Pharmacist { get; set; }

        public int StockId { get; set; }
        public virtual MedicationStock MedicationStock { get; set; }

        public int Quantity { get; set; }

        public bool Deleted { get; set; }

        public int CartId { get; set; }
        public virtual MedicationCart MedicationCart { get; set; }
    }
}
