using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Payment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Display(Name = "Payment method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Amount paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Payment date")]
        public DateTime PaymentDate { get; set; }

        [ForeignKey("PaymentMadeById")]
        public string? PaymentMadeById { get; set; }

        public virtual UserBaseModel PaymentMadeBy { get; set; }
        public PaymentPaymentStatus Status { get; set; }

        public int DeviceInfoId { get; set; }

        public virtual DeviceInfo DeviceInfo { get; set; }

        public string ReferenceNumber { get; set; }
    }

    public enum PaymentMethod
    {
        [Display(Name = "Credit Card")]
        Credit_Card,

        [Display(Name = "Cash")]
        Cash
    }

    public enum PaymentPaymentStatus
    {
        [Display(Name = "UNSUCCESSFUL")]
        Unsuccessful,

        [Display(Name = "SUCCESSFUL")]
        Successful,
    }
}