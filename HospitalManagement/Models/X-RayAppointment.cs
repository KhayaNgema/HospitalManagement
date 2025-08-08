using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class X_RayAppointment : Booking
    {
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public int OriginalBookingId { get; set; }
        [ForeignKey("OriginalBookingId")]
        public virtual Booking Booking { get; set; }

        [DisplayName("Scanner image")]
        public string? ScannerImage { get; set; }

        [DisplayName("Body parts require scanning")]
        public BodyParts BodyParts { get; set; }

        public decimal BookingAmount { get; set; }

        [Display(Name = "Instructions/Notes")]
        public ICollection<string>? Instructions { get; set; } = new List<string>();

        [NotMapped]
        public string InstructionsInput { get; set; }
    }
}
