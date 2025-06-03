using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class PatientBill
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillId { get; set; }

        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public decimal PayableTotalAmount { get; set; }

        public ICollection<PatientBillServices> Services { get; set; }
    }
}
