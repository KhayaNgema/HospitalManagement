using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class OrderViewModel
    {
        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        public string UserId { get; set; }

        public string OrderNumber { get; set; }

        [Display(Name = "Cart Items")]
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Last 4 Digits")]
        public string LastFourDigitsOfOrderNumber
        {
            get
            {
                return !string.IsNullOrEmpty(OrderNumber) && OrderNumber.Length >= 4
                    ? OrderNumber[^4..]
                    : OrderNumber ?? string.Empty;
            }
        }
    }
}
