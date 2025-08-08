using HospitalManagement.Models;

namespace HospitalManagement.Services
{
    public class OrderCalculationService
    {
        public decimal CalculateTotalPrice(List<CartItem> cartItems)
        {
            decimal totalPrice = 0;
            foreach (var cartItem in cartItems)
            {
                totalPrice += cartItem.Quantity * cartItem.MenuItem.Price;
            }
            return totalPrice;
        }
    }
}