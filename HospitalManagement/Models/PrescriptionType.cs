using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum PrescriptionType
    {
        [Display(Name = "Once-off")]
        OnceOff,

        [Display(Name = "Recurring")]
        Recurring
    }
}
