using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class PescriptionRequestViewModel
    {
        public int PescriptionRequestId { get; set; }

        public string PatientFirstName {  get; set; }

        public string PatientLastName { get; set; }

        public string ProfilePicture { get; set; }

        public string PatientIdNumber { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string DoctorFirstName { get; set; }

        public string DoctorLastName { get; set; }

        public string DoctorEmail { get; set; }

        public string DoctorPhoneNumber { get; set; }

        public Specialization DoctorSpecialization{ get; set; }

        public Department DoctorDepartment { get; set; }

        public string RequestDate { get; set; }

        public DateTime? NextCollectionDate { get; set; }

        public DateTime? LastCollectionDate { get; set; }

        public string AccessCode { get; set; }

        public ICollection<Medication> PescribedMedication { get; set; }

        public string AdditionalNotes { get; set; }

        public PrescriptionType? PrescriptionType { get; set; }

        public int? CollectAfterCount { get; set; }

        public DateTime? CollectUntilDate { get; set; }

        public CollectionInterval? CollectInterval { get; set; }

        public MedicationPescriptionStatus Status { get; set; }
    }
}
