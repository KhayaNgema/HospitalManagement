using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class MedicationReminder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReminderId { get; set; }

        public int MedicationPescriptionId { get; set; }
        public virtual MedicationPescription MedicationPescription { get; set; }

        public DateTime SentDate { get; set; }

        public string ReminderMessage { get; set; }

        public DateTime ExpiryDate { get; set; }

        public ReminderStatus Status { get; set; }
    }

    public enum ReminderStatus
    {
        Sent,
        Read,
        Deleted
    }

}
