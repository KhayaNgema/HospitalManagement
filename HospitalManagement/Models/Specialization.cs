using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum Specialization
    {
        [Display(Name = "General Practitioner")]
        GeneralPractitioner,

        [Display(Name = "Cardiologist")]
        Cardiologist,

        [Display(Name = "Dermatologist")]
        Dermatologist,

        [Display(Name = "Neurologist")]
        Neurologist,

        [Display(Name = "Pediatrician")]
        Pediatrician,

        [Display(Name = "Psychiatrist")]
        Psychiatrist,

        [Display(Name = "Orthopedic Surgeon")]
        OrthopedicSurgeon,

        [Display(Name = "Gynecologist")]
        Gynecologist,

        [Display(Name = "Oncologist")]
        Oncologist,

        [Display(Name = "Radiologist")]
        Radiologist,

        [Display(Name = "Urologist")]
        Urologist,

        [Display(Name = "Endocrinologist")]
        Endocrinologist,

        [Display(Name = "Gastroenterologist")]
        Gastroenterologist,

        [Display(Name = "Pulmonologist")]
        Pulmonologist,

        [Display(Name = "Nephrologist")]
        Nephrologist,

        [Display(Name = "Rheumatologist")]
        Rheumatologist,

        [Display(Name = "Ophthalmologist")]
        Ophthalmologist,

        [Display(Name = "ENT Specialist")]
        ENT_Specialist,

        [Display(Name = "Anesthesiologist")]
        Anesthesiologist,

        [Display(Name = "Emergency Medicine")]
        EmergencyMedicine,

        [Display(Name = "Hematologist")]
        Hematologist,

        [Display(Name = "Pathologist")]
        Pathologist,

        [Display(Name = "Allergist/Immunologist")]
        Allergist_Immunologist,

        [Display(Name = "Plastic Surgeon")]
        PlasticSurgeon,

        [Display(Name = "Infectious Disease Specialist")]
        InfectiousDiseaseSpecialist
    }
}
