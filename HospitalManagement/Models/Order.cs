using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Display(Name = "Order number")]
        [ScaffoldColumn(false)]
        public string OrderNumber { get; set; }

        [Display(Name = "Order date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Last updated at")]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Is Paid")]

        public string LastFourDigitsOfOrderNumber
        {
            get
            {
                return OrderNumber.Substring(OrderNumber.Length - 4);
            }
        }

        public bool IsPaid { get; set; }

        [Display(Name = "Customer")]
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual UserBaseModel User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
        }
    }

}
