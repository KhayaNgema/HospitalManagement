using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum CollectionInterval
    {
        [Display(Name = "Day(s)")]
        Days,

        [Display(Name = "Weeks(s)")]
        Weeks,

        [Display(Name = "Months(s)")]
        Months,

        [Display(Name = "Years(s)")]
        Years
    }
}
