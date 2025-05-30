using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum DosageForm
    {
        [Display(Name = "Tablet")]
        Tablet,

        [Display(Name = "Capsule")]
        Capsule,

        [Display(Name = "Injection")]
        Injection,

        [Display(Name = "Syrup")]
        Syrup,

        [Display(Name = "Cream")]
        Cream,

        [Display(Name = "Ointment")]
        Ointment,

        [Display(Name = "Suppository")]
        Suppository,

        [Display(Name = "Drops")]
        Drops,

        [Display(Name = "Inhaler")]
        Inhaler
    }
}
