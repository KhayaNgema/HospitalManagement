using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace Cafeteria.Services
{
    public class CartService
    {
        private const string CartSessionKey = "CartItems";

        public List<CartItemViewModel> GetCartItemsFromSession(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }

            return JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson) ?? new List<CartItemViewModel>();
        }

        public void SaveCartItemsToSession(ISession session, List<CartItemViewModel> items)
        {
            var json = JsonSerializer.Serialize(items);
            session.SetString(CartSessionKey, json);
        }
    }
}
