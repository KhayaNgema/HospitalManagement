using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class NewDeliveryRequestViewModel
    {
        public int PackageId { get; set; }

        public int PescriptionId { get; set; }

        [StringLength(100, ErrorMessage = "Street name cannot exceed 100 characters.")]
        public string? Street { get; set; }

        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [Display(Name = "Province")]
        public Province Province { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
        public string? PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string? Country { get; set; }
    }
}
