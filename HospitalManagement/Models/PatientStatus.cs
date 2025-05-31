using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum PatientStatus
    {
        [Display(Name = "Admitted")]
        Admitted,
        [Display(Name = "Discharged")]
        Discharged,
        [Display(Name = "Failed")]
        Failed,
    }
}
