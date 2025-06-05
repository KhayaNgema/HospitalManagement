using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class PatientBillServices
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientBillServiceId { get; set; }

        public int PatientBillId { get; set; }
        public virtual PatientBill PatientBill { get; set; }

        public int? BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        public int? AdmissionId { get; set; }

        [ForeignKey("AdmissionId")]
        public virtual Admission Admission { get; set; }

        public string ServiceType { get; set; }
        public string ReferenceNumber { get; set; }

        public string ServiceName { get; set; }

        public decimal Subtotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

