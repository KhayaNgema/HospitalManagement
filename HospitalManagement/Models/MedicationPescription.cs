using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationPescription
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicationPescriptionId { get; set; }

        public int? AdmissionId { get; set; }
        public virtual Admission Admission { get; set; }

        public int? BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        [DisplayName("Additional notes")]
        public string? AdditionalNotes { get; set; }

        [DisplayName("Prescribed medication")]
        public ICollection<Medication>? PrescribedMedication { get; set; } = new List<Medication>();

        [DisplayName("Collect after")]
        public int? CollectAfterCount { get; set; }

        [DisplayName("Interval")]
        public CollectionInterval? CollectionInterval { get; set; }

        [DisplayName("Pescription type")]
        public PrescriptionType? PrescriptionType { get; set; }

        public bool? HasDoneCollecting { get; set; }


        [DataType(DataType.Date)]
        public DateTime? LastCollectionDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextCollectionDate { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public UserBaseModel CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedById { get; set; }

        [ForeignKey("UpdatedById")]
        public UserBaseModel ModifiedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? AccessCode { get; set; }

        public byte[]? QrCodeImage { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public MedicationPescriptionStatus Status { get; set; }

        [DisplayName("Reminders")]
        public virtual ICollection<MedicationReminder> Reminders { get; set; } = new List<MedicationReminder>();

    }
}
