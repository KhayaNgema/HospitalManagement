using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Receptionist : UserBaseModel
    {
        [Display(Name = "Years of Experience")]
        [Range(0, 60, ErrorMessage = "Experience must be between 0 and 60 years.")]
        public int YearsOfExperience { get; set; }

        [Display(Name = "Education")]
        [StringLength(200)]
        public string? Education { get; set; }

        [Display(Name = "Biography / About")]
        [StringLength(1000)]
        public string? Biography { get; set; }
    }
}
