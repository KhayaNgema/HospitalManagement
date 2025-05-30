using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class UserBaseModel : IdentityUser
    {
        public UserBaseModel()
        {
            ProfilePicture = "UploadedFiles/default_profile_image.jpg";
        }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Profile picture URL cannot exceed 100 characters.")]
        public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "ID number is required.")]
        [StringLength(20, ErrorMessage = "ID number cannot exceed 20 characters.")]
        public string IdNumber { get; set; }

        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Phone(ErrorMessage = "Invalid alternate phone number format.")]
        public string? AlternatePhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

/*        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State name cannot exceed 100 characters.")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
        public string? PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string? Country { get; set; }*/

        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsFirstTimeLogin { get; set; }

    }
}
