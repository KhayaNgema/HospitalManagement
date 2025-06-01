using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class OrderItemViewModel
    {
        [Display(Name = "Menu Item ID")]
        public int MenuItemId { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Subtotal")]
        public decimal SubTotal { get; set; }

        [Display(Name = "Menu Item Name")]
        public string MenuItemName { get; set; }

        [Display(Name = "Menu Item Price")]
        public decimal MenuItemPrice { get; set; }
    }

}
