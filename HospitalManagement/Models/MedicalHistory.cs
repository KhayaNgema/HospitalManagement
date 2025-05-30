using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicalHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicalHistoryId { get; set; }

        public string PatientId{ get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }    

        [Display(Name = "Allergies")]
        public string? Allergies { get; set; }

        [Display(Name = "Previous Illnesses")]
        public string? PreviousIllnesses { get; set; }

        [Display(Name = "Current Medications")]
        public string? CurrentMedications { get; set; }

        [Display(Name = "Operations or Surgeries")]
        public string? Surgeries { get; set; }

        [Display(Name = "Blood Group")]
        public BloodType BloodGroup { get; set; }

        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
