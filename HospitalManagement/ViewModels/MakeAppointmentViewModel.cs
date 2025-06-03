using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MakeAppointmentViewModel
    {
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime BookForDate { get; set; }

        [Required]
        [Display(Name = "Medical Condition")]
        public CommonMedicalCondition MedicalCondition { get; set; }

        [Required]
        [Display(Name = "Select Time Slot")]
        public int SelectedTimeSlotId { get; set; }

        [Required(ErrorMessage = "Please select a time slot for the appointment.")]
        public string BookForTimeSlot { get; set; }

        public List<SelectListItem> AvailableTimeSlots { get; set; }

        public string? AdditionalNotes { get; set; }

        public string PatientId { get; set; }
    }

}


