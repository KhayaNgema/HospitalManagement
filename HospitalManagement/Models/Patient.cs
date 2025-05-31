using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Patient : UserBaseModel
    {
        [StringLength(10, ErrorMessage = "Blood group cannot exceed 10 characters.")]
        public BloodType? BloodType { get; set; }

        [StringLength(100, ErrorMessage = "Allergies cannot exceed 100 characters.")]
        public string? Allergies { get; set; }
    }
}
