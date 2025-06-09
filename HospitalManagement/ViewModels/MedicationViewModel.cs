using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MedicationViewModel
    {
        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string? MedicationImage { get; set; }

        public DosageForm DosageForm { get; set; }

        public Strength Strength { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public IFormFile MedicationImages { get; set; }

        public int CategoryId { get; set; }
    }
}
