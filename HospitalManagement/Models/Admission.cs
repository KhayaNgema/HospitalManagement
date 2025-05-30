using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Admission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdmissionId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Patient Patient { get; set; }    

        [DataType(DataType.Date, ErrorMessage = "Invalid admission date format.")]
        public DateTime? AdmissionDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid discharge date format.")]
        public DateTime? DischargeDate { get; set; }

        [StringLength(50, ErrorMessage = "Ward name cannot exceed 50 characters.")]
        public string? Ward { get; set; }

        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters.")]
        public string? RoomNumber { get; set; }

        [StringLength(10, ErrorMessage = "Bed number cannot exceed 10 characters.")]
        public string? BedNumber { get; set; }

        public int? AttendingPhysicianId { get; set; }

        [StringLength(50, ErrorMessage = "Department name cannot exceed 50 characters.")]
        public string? Department { get; set; }
        public string? PatientStatus { get; set; }

        public int MedicalHistoryId { get; set; }
        public virtual MedicalHistory MedicalHistory { get; set; }

        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters.")]
        public string? CurrentDiagnosis { get; set; }

        [StringLength(500, ErrorMessage = "Medications cannot exceed 500 characters.")]
        public string? CurrentMedications { get; set; }

        [StringLength(500, ErrorMessage = "Surgeries description cannot exceed 500 characters.")]
        public string? Surgeries { get; set; }

        [StringLength(500, ErrorMessage = "Immunizations description cannot exceed 500 characters.")]
        public string? Immunizations { get; set; }
        public float? HeightCm { get; set; }
        public float? WeightKg { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastVisitDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextAppointmentDate { get; set; }

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public UserBaseModel CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
