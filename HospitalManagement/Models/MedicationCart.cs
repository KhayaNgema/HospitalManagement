using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HospitalManagement.Models
{
    public class MedicationCart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<MedicationCartItem> Items { get; set; }

        public MedicationCart()
        {
            CreatedAt = DateTime.Now;
            Items = new List<MedicationCartItem>();
        }

        public void AddItem(MedicationStock medicationStock, int quantity)
        {
            MedicationCartItem existingItem = Items.FirstOrDefault(item => item.MedicationStock.StockId == medicationStock.StockId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new MedicationCartItem { MedicationStock = medicationStock, Quantity = quantity });
            }
        }

        public void RemoveItem(int medicationStockId, IHttpContextAccessor httpContextAccessor)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.MedicationStock.StockId == medicationStockId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                var session = httpContextAccessor.HttpContext!.Session;
                session.SetString("Cart", JsonSerializer.Serialize(this));
            }
        }

        public void UpdateQuantity(int medicationStockId, int quantity)
        {
            var itemToUpdate = Items.FirstOrDefault(item => item.MedicationStock.StockId == medicationStockId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

    }
}
