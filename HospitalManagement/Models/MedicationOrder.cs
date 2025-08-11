using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Display(Name = "Order number")]
        [ScaffoldColumn(false)]
        public string OrderNumber { get; set; }

        [Display(Name = "Order date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Last updated at")]
        public DateTime LastUpdated { get; set; }

        public string LastFourDigitsOfOrderNumber
        {
            get
            {
                return OrderNumber.Substring(OrderNumber.Length - 4);
            }
        }

        [Display(Name = "Pharmacist")]
        public string PharmacistId { get; set; }
        [ForeignKey("PharmacistId")]
        public virtual Pharmacist Pharmacist { get; set; }

        [Display(Name = "Received By")]
        public string? ReceivedById { get; set; }
        [ForeignKey("ReceivedById")]
        public virtual Pharmacist ReceivedBy { get; set; }

        public virtual ICollection<MedicationOrderItem> OrderItems { get; set; }

        public MedicationOrder()
        {
            OrderDate = DateTime.Now;
        }
    }

}
