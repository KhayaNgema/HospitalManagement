using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Ready for collection")]
        Ready_For_Collection,

        [Display(Name = "Collected")]
        Collected,

        [Display(Name = "Cancelled")]
        Cancelled
    }
}
