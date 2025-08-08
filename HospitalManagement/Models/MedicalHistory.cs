using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicalHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicalHistoryId { get; set; }

        [Required]
        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public int? PatientMedicalHistoryId { get; set; }

        [ForeignKey("PatientMedicalHistoryId")]
        public virtual PatientMedicalHistory PatientMedicalHistory { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [Display(Name = "Chief Complaint")]
        public string? ChiefComplaint { get; set; }

        [Display(Name = "Symptoms")]
        public List<string>? Symptoms { get; set; }

        [Display(Name = "Diagnosis")]
        public string? Diagnosis { get; set; }

        public List<string>? Surgeries { get; set; }

        public List<string>? Immunizations { get; set; }
        public float? HeightCm { get; set; }
        public float? WeightKg { get; set; }

        [Display(Name = "Pescribed medication")]
        public ICollection<Medication> PrescribedMedication { get; set; }

        [Display(Name = "Treatment Given")]
        public string? Treatment { get; set; }

        [Display(Name = "Medications Prescribed")]
        public virtual ICollection<Medication>? Medications { get; set; }

        [Display(Name = "Follow-Up Instructions")]
        public List<string>? FollowUpInstructions { get; set; }

        [Display(Name = "Vital Signs")]
        public string? Vitals { get; set; }

        [Display(Name = "Lab Results Summary")]
        public string? LabResults { get; set; }

        [Display(Name = "Doctor's Notes")]
        public string? Notes { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [DisplayName("Until date")]
        public DateTime? UntilDate { get; set; }

        [DisplayName("Collect after")]
        public int? CollectAfterCount { get; set; }

        [DisplayName("Interval")]
        public CollectionInterval? CollectionInterval { get; set; }

        [DisplayName("Pescription type")]
        public PrescriptionType PrescriptionType { get; set; }


        [Display(Name = "Recorded On")]
        public DateTime RecordedAt { get; set; } = DateTime.Now;
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
