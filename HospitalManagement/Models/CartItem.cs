using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class CartItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        public string PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public int MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }

        public bool Deleted { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
