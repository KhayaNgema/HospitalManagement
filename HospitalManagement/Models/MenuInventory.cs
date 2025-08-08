using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MenuInventory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuInventoryId { get; set; }

        public int ItemId { get; set; }

        public int Quantity { get; set; }

        public ItemAvailability Availability { get; set; }
    }
}
