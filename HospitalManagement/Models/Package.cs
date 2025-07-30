using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Package
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageId { get; set; }

        public int DeliveryRequestId { get; set; }
        public virtual DeliveryRequest DeliveryRequest { get; set; }

        public string DeliveryPersonnelId { get; set; }
        [ForeignKey("DeliveryPersonnelId")]
        public virtual DeliveryPersonnel DeliveryPersonnel { get; set; }

        public int? DeliveryId { get; set; }

        [ForeignKey("DeliveryId")]
        public virtual Delivery Delivery { get; set; }

        public DateTime CollectionDate { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Pharmacist CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }

        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
