using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MedicationCartItemViewModel
    {
        [Required(ErrorMessage = "Menu item Id is required")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Menu item name is required")]
        public string MedicationName { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
    }
}
