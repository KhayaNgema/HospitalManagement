using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class XRayAppointmentDetailsViewModel
    {
        public int BookingId { get; set; }

        public int OriginalBookingId { get; set; }

        public string BookingReference { get; set; }

        public string PatientId { get; set; }
        public string PatientFullNames{ get; set; }

        public string PatientProfilePicture {  get; set; }

        public string PatientEmail { get; set; }

        public string PhoneNumber { get; set; }


        public string DoctorId { get; set; }

        public string DoctorFullNames { get; set; }



        public DateTime BookForDate { get; set; }

        public DateTime BookForTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public BookingStatus Status { get; set; }

        public CommonMedicalCondition MedicalCondition { get; set; }

        public string  AdditionalNotes {  get; set; }
        public ICollection<string>? Instructions { get; set; } = new List<string>();

        public ICollection<Specialization> Specialization { get; set; }

        public IFormFile XRayImages { get; set; }

        public string XRayImage { get; set; }
    }
}
