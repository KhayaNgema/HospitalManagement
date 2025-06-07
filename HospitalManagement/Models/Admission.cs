using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Admission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdmissionId { get; set; }

        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }


        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid admission date format.")]
        public DateTime? AdmissionDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid discharge date format.")]
        public DateTime? DischargeDate { get; set; }

        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters.")]
        public string? RoomNumber { get; set; }

        [StringLength(10, ErrorMessage = "Bed number cannot exceed 10 characters.")]
        public string? BedNumber { get; set; }

        public Department Department { get; set; }
        public PatientStatus PatientStatus { get; set; }

        public int PatientMedicalHistoryId { get; set; }
        public virtual PatientMedicalHistory MedicalHistory { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastVisitDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextAppointmentDate { get; set; }

        [DisplayName("Additional notes")]
        public string? AdditionalNotes { get; set; }

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public Doctor CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
