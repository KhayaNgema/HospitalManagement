using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        public string PatientId { get; set; } 

        [ForeignKey("UserId")]
        public virtual Patient Patient { get; set; }

        [Required(ErrorMessage = "Please select a date for the appointment.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime BookForDate { get; set; }

        [Required(ErrorMessage = "Please select a time for the appointment.")]
        [DataType(DataType.Time)]
        [Display(Name = "Appointment Time")]
        public DateTime BookForTime { get; set; }

        [Required(ErrorMessage = "Please select the medical condition.")]
        public CommonMedicalCondition MedicalCondition { get; set; }

        [Display(Name = "Booking Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public UserBaseModel CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public byte[]? QrCodeImage { get; set; }

        public string BookingReference { get; set; }

        [Display(Name = "Additional notes")]
        public string? AdditionalNotes { get; set; }
    }
}
