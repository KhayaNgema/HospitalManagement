using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum ItemAvailability
    {
        [Display(Name = "Available")]
        Available,

        [Display(Name = "Out of Stock")]
        OutOfStock
    }
}
