using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationOrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public virtual MedicationOrder MedicationOrder { get; set; }

        public int StockId { get; set; }

        public virtual MedicationStock MedicationStock { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
