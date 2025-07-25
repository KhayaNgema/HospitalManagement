using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;

namespace HospitalManagement.Areas.Identity.Pages.Account
{
    public class RegisterKitchenStaffModel : PageModel
    {
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly FileUploadService _fileUploadService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RandomPasswordGeneratorService _passwordGenerator;
        private readonly IEmailSender _emailSender;
        private readonly EmailService _emailService;
        private readonly HospitalManagementDbContext _context;
        private readonly IActivityLogger _activityLogger;

        public RegisterKitchenStaffModel(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            SignInManager<UserBaseModel> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            FileUploadService fileUploadService,
            RoleManager<IdentityRole> roleManager,
            RandomPasswordGeneratorService passwordGenerator,
            EmailService emailService,
            HospitalManagementDbContext db,
            IActivityLogger activityLogger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _roleManager = roleManager;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _context = db;
            _activityLogger = activityLogger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Date of birth")]
            public DateTime DateOfBirth { get; set; }


            [Display(Name = "Profile picture")]
            public IFormFile? ProfilePicture { get; set; }

            [Required]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Alternate phone number")]
            public string AlternatePhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [StringLength(100, ErrorMessage = "Street name cannot exceed 100 characters.")]
            public string? Street { get; set; }

            [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
            public string? City { get; set; }

            [Required(ErrorMessage = "Province is required.")]
            [Display(Name = "Province")]
            public Province Province { get; set; }

            [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
            public string? PostalCode { get; set; }

            [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
            public string? Country { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required]
            [Display(Name = "Identity number")]
            public string IdNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            try
            {
                var existingUserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == Input.PhoneNumber);
                if (existingUserByPhoneNumber != null)
                {
                    ModelState.AddModelError("Input.PhoneNumber", "An account with this phone number already exists.");
                    return Page();
                }

                var existingUserByEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Input.Email", "An account with this email address already exists.");

                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);

                var doctor = new KitchenStaff
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    DateOfBirth = Input.DateOfBirth,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    CreatedBy = $"{user.FirstName} {user.LastName}",
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = $"{user.FirstName} {user.LastName}",
                    ModifiedDateTime = DateTime.Now,
                    IsActive = true,
                    IsSuspended = false,
                    IsFirstTimeLogin = true,
                    AccessFailedCount = 0,
                    AlternatePhoneNumber = Input.AlternatePhoneNumber,
                    Address = string.Join(", ", new[] {
                              Input.Street, Input.City, Input.Province.ToString(), Input.PostalCode, Input.Country
                                                       }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    Gender = Input.Gender,
                    IdNumber = Input.IdNumber,
                    IsDeleted = false,
                };

                if (Input.ProfilePicture != null && Input.ProfilePicture.Length > 0)
                {
                    var playerProfilePicturePath = await _fileUploadService.UploadFileAsync(Input.ProfilePicture);
                    doctor.ProfilePicture = playerProfilePicturePath;
                }

                await _userStore.SetUserNameAsync(doctor, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(doctor, Input.Email, CancellationToken.None);

                string randomPassword = _passwordGenerator.GenerateRandomPassword();
                var result = await _userManager.CreateAsync(doctor, randomPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Doctor a new account with password.");

                    await _userManager.AddToRoleAsync(doctor, "Kitchen Staff");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(doctor);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = doctor.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    string accountCreationEmailBody = $"Hello {doctor.FirstName},<br><br>";
                    accountCreationEmailBody += $"Welcome to MediCare!<br><br>";
                    accountCreationEmailBody += $"You have been successfully added as Medicare kitchen staff. Below are your login credentials:<br><br>";
                    accountCreationEmailBody += $"Email: {doctor.Email}<br>";
                    accountCreationEmailBody += $"Password: {randomPassword}<br><br>";
                    accountCreationEmailBody += $"Please note that we have sent you two emails, including this one. You need to open the other email to confirm your email address before you can log into the system.<br><br>";
                    accountCreationEmailBody += $"Thank you!";

                    BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(doctor.Email, "Confirm Your Email Address", accountCreationEmailBody, "MediCare"));

                    string emailConfirmationEmailBody = $"Hello {doctor.FirstName},<br><br>";
                    emailConfirmationEmailBody += $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br><br>";
                    emailConfirmationEmailBody += $"Thank you!";

                    BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(doctor.Email, "Confirm Your Email Address", emailConfirmationEmailBody, "Diski 360"));


                    TempData["Message"] = $"{doctor.FirstName} {doctor.LastName}  has been successfully added as your new kitchen staff";
                    return RedirectToAction("Doctors", "Users");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to onboard doctor: " + ex.Message);

                return Page();
            }


            return Page();
        }

        private IUserEmailStore<UserBaseModel> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserBaseModel>)_userStore;
        }
    }
}
