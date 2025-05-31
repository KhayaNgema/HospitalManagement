using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class AdmitPatientViewModel
    {
        public string PatientId { get; set; }

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

        [StringLength(50, ErrorMessage = "Department name cannot exceed 50 characters.")]
        public Department Department { get; set; }
        public PatientStatus PatientStatus { get; set; }
        public int MedicalHistoryId { get; set; }

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
    }
}
