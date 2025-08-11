using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class DeliveryPackageItem
    {
        [Key]
        public int DeliveryPackageItemId { get; set; }

        public int DeliveryRequestId { get; set; }

        [ForeignKey(nameof(DeliveryRequestId))]
        public DeliveryRequest DeliveryRequest { get; set; }

        public int MedicationId { get; set; }

        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; }

        public bool IsPackaged { get; set; } = false;

        public bool IsCollected { get; set; } = false;

        public DateTime? PackagedAt { get; set; }

        public DateTime? CollectionAt { get; set; }
    }
}
