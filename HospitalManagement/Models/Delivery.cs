using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Delivery
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryId { get; set; }

        public string Address { get; set; }

        public decimal PatientLatitude { get; set; }
        public decimal PatientLongitude { get; set; }

        public decimal DriverLatitude { get; set; }
        public decimal DriverLongitude { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; }

        public int PackageId  { get; set; }
        public Package Package { get; set; }
    }
}
