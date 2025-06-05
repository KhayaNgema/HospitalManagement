using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Medication
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string? MedicationImage { get; set; }

   
        public DosageForm DosageForm { get; set; } 

        public Strength Strength { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; } 

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public bool IsExpired { get; set; }

        public bool IsPrescriptionRequired { get; set; }

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public UserBaseModel CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public virtual ICollection<MedicationPescription> MedicationPescriptions { get; set; } = new List<MedicationPescription>();
    }
}
