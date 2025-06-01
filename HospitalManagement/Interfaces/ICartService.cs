/*using HospitalManagement.ViewModels;
using System.Collections.Generic;
using System.Web;

public interface ICartService
{
    List<CartItemViewModel> GetCartItemsFromSession(ISession session);
}


public class CartService : ICartService
{
    public List<CartItemViewModel> GetCartItemsFromSession(ISession session)
    {
        return session["Cart"] as List<CartItemViewModel> ?? new List<CartItemViewModel>();
    }
}*/