using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class MakeAppointmentViewModel
    {
        [Required]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Please select a date for the appointment.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime BookForDate { get; set; }

        [Required(ErrorMessage = "Please select a time for the appointment.")]
        [DataType(DataType.Time)]
        [Display(Name = "Appointment Time")]
        public DateTime BookForTime { get; set; }

        [Required(ErrorMessage = "Please select the medical condition.")]
        public CommonMedicalCondition MedicalCondition { get; set; }
    }
}
