using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class MenuViewModel
    {
        public List<MenuItem> MenuItems { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
