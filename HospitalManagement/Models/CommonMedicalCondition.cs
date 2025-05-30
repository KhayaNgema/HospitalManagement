using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public enum CommonMedicalCondition
    {
        [Display(Name = "Fever")] Fever,
        [Display(Name = "Cold and Flu")] ColdAndFlu,
        [Display(Name = "Headache")] Headache,
        [Display(Name = "Stomach Pain")] StomachPain,
        [Display(Name = "High Blood Pressure")] HighBloodPressure,
        [Display(Name = "Diabetes")] Diabetes,
        [Display(Name = "Asthma")] Asthma,
        [Display(Name = "Allergy")] Allergy,
        [Display(Name = "Back Pain")] BackPain,
        [Display(Name = "Skin Rash")] SkinRash,
        [Display(Name = "Infection")] Infection,
        [Display(Name = "Anxiety")] Anxiety,
        [Display(Name = "Depression")] Depression,
        [Display(Name = "Chest Pain")] ChestPain,
        [Display(Name = "Arthritis")] Arthritis,
        [Display(Name = "Migraine")] Migraine,
        [Display(Name = "Fatigue")] Fatigue,
        [Display(Name = "Throat Infection")] ThroatInfection,
        [Display(Name = "Eye Irritation")] EyeIrritation,
        [Display(Name = "Ear Pain")] EarPain
    }
}
