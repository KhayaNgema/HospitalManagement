using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Sub total")]
        public decimal SubTotal { get; set; }


        public string MenuItemName { get; set; }

        public decimal MenuItemPrice { get; set; }
    }
}
