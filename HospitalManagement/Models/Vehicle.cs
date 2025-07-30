using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public VehicleType Type { get; set; }
        public VehicleMake Make { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }

        public string DeliveryPersonnelId { get; set; }
        [ForeignKey("DeliveryPersonnelId")]
        public virtual DeliveryPersonnel DeliveryPersonnel { get; set; }
    }
}
