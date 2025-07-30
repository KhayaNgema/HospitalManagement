using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class DeliveryRequest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryRequestId { get; set; }

        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public int MedicationPescriptionId { get; set; }

        [ForeignKey("MedicationPescriptionId")]
        public virtual MedicationPescription MedicationPescription { get; set; }

        public string Address { get; set; }

        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public DeliveryRequestStatus Status { get; set; }
    }
}
