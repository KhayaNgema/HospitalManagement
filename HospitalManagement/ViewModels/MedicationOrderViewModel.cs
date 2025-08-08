using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MedicationOrderViewModel
    {
        [Display(Name = "Cart Items")]
        public List<MedicationCartItem> MedicationCartItems { get; set; } = new List<MedicationCartItem>();
    }
}
