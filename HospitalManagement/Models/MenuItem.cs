using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MenuItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Item name")]
        public string ItemName { get; set; }

        [Required]
        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Item image(s)")]
        public string ItemImage { get; set; }

        [Required]
        [Display(Name = "Item price")]
        public decimal Price { get; set; }

        [Display(Name = "Is selected")]
        public bool IsSelected { get; set; }


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
