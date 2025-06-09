using HospitalManagement.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MedicalHistoryViewModel
    {
        public int BookingId { get; set; }

        [Required]
        public string PatientId { get; set; }

        public int PatientMedicalHistoryId { get; set; }

        [Display(Name = "Chief Complaint")]
        public string? ChiefComplaint { get; set; }

        [Display(Name = "Symptoms")]
        public List<string>? Symptoms { get; set; }

        [Display(Name = "Diagnosis")]
        public string? Diagnosis { get; set; }

        public List<string>? Surgeries { get; set; }

        public List<string>? Immunizations { get; set; }

        [Display(Name = "Height (cm)")]
        public float? HeightCm { get; set; }

        [Display(Name = "Weight (kg)")]
        public float? WeightKg { get; set; }

        [Display(Name = "Treatment Given")]
        public string? Treatment { get; set; }


        [Display(Name = "Follow-Up Instructions")]
        public List<string>? FollowUpInstructions { get; set; }

        [Display(Name = "Vital Signs")]
        public string? Vitals { get; set; }

        [Display(Name = "Lab Results Summary")]
        public string? LabResults { get; set; }

        [Display(Name = "Doctor's Notes")]
        public string? Notes { get; set; }
        public string DoctorId { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Profile picture URL cannot exceed 100 characters.")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Pescribed medication")]
        public ICollection<Medication> PrescribedMedication { get; set; }

        [DisplayName("Until date")]
        public DateTime? UntilDate { get; set; }

        [DisplayName("Collect after")]
        public int? CollectAfterCount { get; set; }

        [DisplayName("Interval")]
        public CollectionInterval? CollectionInterval { get; set; }

        [DisplayName("Pescription type")]
        public PrescriptionType PrescriptionType { get; set; }
    }
}
