using HospitalManagement.Models;

namespace HospitalManagement.Helpers
{
    public static class ConditionToSpecializationsMap
    {
        public static readonly Dictionary<CommonMedicalCondition, List<Specialization>> Map = new()
    {
        { CommonMedicalCondition.Fever, new() { Specialization.GeneralPractitioner, Specialization.Pediatrician } },
        { CommonMedicalCondition.ColdAndFlu, new() { Specialization.GeneralPractitioner, Specialization.Pediatrician } },
        { CommonMedicalCondition.Headache, new() { Specialization.GeneralPractitioner, Specialization.Neurologist } },
        { CommonMedicalCondition.StomachPain, new() { Specialization.GeneralPractitioner, Specialization.Gastroenterologist, Specialization.Gynecologist } },
        { CommonMedicalCondition.HighBloodPressure, new() { Specialization.Cardiologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Diabetes, new() { Specialization.Endocrinologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Asthma, new() { Specialization.Pulmonologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Allergy, new() { Specialization.Allergist_Immunologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.BackPain, new() { Specialization.OrthopedicSurgeon, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.SkinRash, new() { Specialization.Dermatologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Infection, new() { Specialization.InfectiousDiseaseSpecialist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Anxiety, new() { Specialization.Psychiatrist } },
        { CommonMedicalCondition.Depression, new() { Specialization.Psychiatrist } },
        { CommonMedicalCondition.ChestPain, new() { Specialization.Cardiologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Arthritis, new() { Specialization.Rheumatologist, Specialization.OrthopedicSurgeon } },
        { CommonMedicalCondition.Migraine, new() { Specialization.Neurologist, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.Fatigue, new() { Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.ThroatInfection, new() { Specialization.ENT_Specialist, Specialization.Pediatrician, Specialization.GeneralPractitioner } },
        { CommonMedicalCondition.EyeIrritation, new() { Specialization.Ophthalmologist } },
        { CommonMedicalCondition.EarPain, new() { Specialization.ENT_Specialist } },
    };
    }
}
