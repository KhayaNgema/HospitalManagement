using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum Province
    {
        [Display(Name = "Eastern Cape")]
        EasternCape,

        [Display(Name = "Free State")]
        FreeState,

        [Display(Name = "Gauteng")]
        Gauteng,

        [Display(Name = "KwaZulu-Natal")]
        KwaZuluNatal,

        [Display(Name = "Limpopo")]
        Limpopo,

        [Display(Name = "Mpumalanga")]
        Mpumalanga,

        [Display(Name = "Northern Cape")]
        NorthernCape,

        [Display(Name = "North West")]
        NorthWest,

        [Display(Name = "Western Cape")]
        WesternCape
    }
}

