using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Category Id")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Category name")]
        public string CategoryName { get; set; }
    }
}
