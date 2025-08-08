using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationUsageLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public int MedicationId { get; set; }
        public virtual Medication Medication { get; set; }

        public DateTime DispensedOn { get; set; }

        public int QuantityDispensed { get; set; }

        public int? MedicationPescriptionId { get; set; }
        public virtual MedicationPescription MedicationPescription { get; set; }

        public string DispensedById { get; set; }
        [ForeignKey("DispensedById")]
        public UserBaseModel DispensedBy { get; set; }

        public string? Notes { get; set; }
    }
}
