using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MenuItemViewModel
    {
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

        [Display(Name = "Item image")]
        public IFormFile ItemImages { get; set; }

    }
}
