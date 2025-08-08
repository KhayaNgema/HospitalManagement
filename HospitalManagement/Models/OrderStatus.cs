using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Ready for collection")]
        Ready_For_Delivery,

        [Display(Name = "Packaged")]
        Packaged,

        [Display(Name = "On the way")]
        OnTheWay,

        [Display(Name = "Delivered")]
        Collected,

        [Display(Name = "Cancelled")]
        Cancelled
    }
}
