using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum Strength
    {
        [Display(Name = "100 mg")]
        Mg100,

        [Display(Name = "250 mg")]
        Mg250,

        [Display(Name = "500 mg")]
        Mg500,

        [Display(Name = "1 g")]
        G1,

        [Display(Name = "5 mg/ml")]
        MgPerMl5,

        [Display(Name = "10 mg/ml")]
        MgPerMl10
    }
}
