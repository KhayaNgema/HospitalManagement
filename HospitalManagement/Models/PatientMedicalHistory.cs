using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class PatientMedicalHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientMedicalHistoryId { get; set; }

        [Required]
        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public virtual ICollection<MedicalHistory> MedicalHistories { get; set; }

        public string? AccessCode { get; set; }

        public byte[]? QrCodeImage { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public bool IsActive => DateTime.Now < ExpiresAt;
    }
}
