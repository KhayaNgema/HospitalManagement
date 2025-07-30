using HospitalManagement.Models;
using HospitalManagement.ViewModels;

namespace HospitalManagement.ViewModels
{

    public class MedicationRemindersWithDeliveryViewModel
    {
        public List<MedicationReminder> Reminders { get; set; }
        public Dictionary<int, NewDeliveryRequestViewModel> DeliveryRequests { get; set; }
    }

}


