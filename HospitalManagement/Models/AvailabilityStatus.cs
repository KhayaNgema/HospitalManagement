using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum AvailabilityStatus
    {
        [Display(Name = "Available")]
        Available,

        [Display(Name = "Unavailable")]
        Unavailable,

        [Display(Name = "On Leave")]
        OnLeave,

        [Display(Name = "Busy with patient")]
        BusyWithPatient,

        [Display(Name = "In Surgery")]
        InSurgery,

        [Display(Name = "Off Duty")]
        OffDuty
    }
}
