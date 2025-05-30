using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace HospitalManagement.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<CartItem> Items { get; set; }

        public Cart()
        {
            CreatedAt = DateTime.Now;
            Items = new List<CartItem>();
        }

        public void AddItem(MenuItem menuItem, int quantity)
        {
            CartItem existingItem = Items.FirstOrDefault(item => item.MenuItem.ItemId == menuItem.ItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem { MenuItem = menuItem, Quantity = quantity });
            }
        }

        public void RemoveItem(int menuItemId, IHttpContextAccessor httpContextAccessor)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.MenuItem.ItemId == menuItemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                var session = httpContextAccessor.HttpContext!.Session;
                session.SetString("Cart", JsonSerializer.Serialize(this));
            }
        }

        public void UpdateQuantity(int menuItemId, int quantity)
        {
            var itemToUpdate = Items.FirstOrDefault(item => item.MenuItem.ItemId == menuItemId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public decimal CalculateTotal()
        {
            return Items.Sum(item => Convert.ToDecimal(item.MenuItem.Price) * item.Quantity);
        }

    }
}
