using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum PrescriptionType
    {
        [Display(Name = "Once-off")]
        Once_Off,

        [Display(Name = "Recurring")]
        Recurring
    }
}
