﻿using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class BookXRayViewModel
    {
        [Required]
        public string PatientId { get; set; }

        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please select a date for the appointment.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime BookForDate { get; set; }

        [Required(ErrorMessage = "Please select a time slot for the appointment.")]
        public string BookForTimeSlot { get; set; }

        public List<SelectListItem> AvailableTimeSlots { get; set; }

        [Required(ErrorMessage = "Please select the medical condition.")]
        public CommonMedicalCondition MedicalCondition { get; set; }

        [Display(Name = "Additional notes")]
        public string? AdditionalNotes { get; set; }

        [DisplayName("Scanner image")]
        public string? ScannerImage { get; set; }

        [DisplayName("Body parts require scanning")]
        public BodyParts BodyParts { get; set; }

        [Display(Name = "Instructions/Notes")]
        public ICollection<string>? Instructions { get; set; }

        public string InstructionsInput { get; set; }

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
