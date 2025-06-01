using HospitalManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class AdmitPatientViewModel
    {
        public int BookingId { get; set; }
        public string PatientId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid admission date format.")]
        public DateTime? AdmissionDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid discharge date format.")]
        public DateTime? DischargeDate { get; set; }

        public Ward Ward { get; set; }

        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters.")]
        public string? RoomNumber { get; set; }

        [StringLength(10, ErrorMessage = "Bed number cannot exceed 10 characters.")]
        public string? BedNumber { get; set; }

        public Department Department { get; set; }
        public PatientStatus PatientStatus { get; set; }
        public int PatientMedicalHistoryId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastVisitDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextAppointmentDate { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Profile picture URL cannot exceed 100 characters.")]
        public string? ProfilePicture { get; set; }

        [StringLength(20, ErrorMessage = "ID number cannot exceed 20 characters.")]
        public string? IdNumber { get; set; }

        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AlternatePhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }
    }
}
