using System.ComponentModel.DataAnnotations;


namespace HospitalManagement.Models
{
    public enum Department
    {
        [Display(Name = "Emergency Department")]
        EmergencyDepartment,

        [Display(Name = "Outpatient Department")]
        OutpatientDepartment,

        [Display(Name = "Inpatient / Wards")]
        Inpatient,

        [Display(Name = "Intensive Care Unit")]
        ICU,

        [Display(Name = "Neonatal ICU")]
        NICU,

        [Display(Name = "Pediatric ICU")]
        PICU,

        [Display(Name = "Cardiac ICU")]
        CCU,

        [Display(Name = "Operating Theaters / Surgery")]
        Surgery,

        [Display(Name = "Obstetrics & Maternity")]
        Obstetrics,

        [Display(Name = "Gynecology")]
        Gynecology,

        [Display(Name = "Pediatrics")]
        Pediatrics,

        [Display(Name = "Internal Medicine")]
        InternalMedicine,

        [Display(Name = "General Surgery")]
        GeneralSurgery,

        [Display(Name = "Orthopedics")]
        Orthopedics,

        [Display(Name = "Cardiology")]
        Cardiology,

        [Display(Name = "Neurology")]
        Neurology,

        [Display(Name = "Neurosurgery")]
        Neurosurgery,

        [Display(Name = "Urology")]
        Urology,

        [Display(Name = "Dermatology")]
        Dermatology,

        [Display(Name = "Ophthalmology (Eye Department)")]
        Ophthalmology,

        [Display(Name = "ENT (Ear, Nose, Throat)")]
        ENT,

        [Display(Name = "Psychiatry / Mental Health")]
        Psychiatry,

        [Display(Name = "Oncology (Cancer Care)")]
        Oncology,

        [Display(Name = "Gastroenterology")]
        Gastroenterology,

        [Display(Name = "Pulmonology (Respiratory Medicine)")]
        Pulmonology,

        [Display(Name = "Nephrology (Kidney Care)")]
        Nephrology,

        [Display(Name = "Endocrinology (Diabetes, Thyroid, Hormones)")]
        Endocrinology,

        [Display(Name = "Infectious Diseases")]
        InfectiousDiseases,

        [Display(Name = "Radiology / Imaging")]
        Radiology,

        [Display(Name = "Laboratory / Pathology")]
        Laboratory,

        [Display(Name = "Pharmacy")]
        Pharmacy,

        [Display(Name = "Physiotherapy / Rehabilitation")]
        Physiotherapy,

        [Display(Name = "Nutrition & Dietetics")]
        Nutrition,

        [Display(Name = "Occupational Therapy")]
        OccupationalTherapy,

        [Display(Name = "Speech Therapy")]
        SpeechTherapy,

        [Display(Name = "Social Work / Counseling")]
        SocialWork,

        [Display(Name = "Medical Records / Health Information")]
        MedicalRecords,

        [Display(Name = "Billing & Finance")]
        Billing,

        [Display(Name = "Human Resources")]
        HR,

        [Display(Name = "Information Technology")]
        IT,

        [Display(Name = "Facilities / Maintenance")]
        Facilities,

        [Display(Name = "Housekeeping / Environmental Services")]
        Housekeeping,

        [Display(Name = "Security")]
        Security,

        [Display(Name = "Laundry Services")]
        Laundry,

        [Display(Name = "Transport / Patient Transfer")]
        Transport,

        [Display(Name = "Quality Assurance")]
        QualityAssurance,

        [Display(Name = "Infection Control")]
        InfectionControl,

        [Display(Name = "Medical Education & Training")]
        MedicalEducation
    }

}
