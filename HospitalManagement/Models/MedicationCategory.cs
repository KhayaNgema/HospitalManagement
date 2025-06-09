using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class MedicationCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Category Id")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Category name")]
        public string CategoryName { get; set; }

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
