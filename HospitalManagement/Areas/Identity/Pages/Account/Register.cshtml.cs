// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.Data;

namespace HospitalManagement.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly FileUploadService _fileUploadService;
        private readonly HospitalManagementDbContext _context;

        public RegisterModel(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            SignInManager<UserBaseModel> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            FileUploadService fileUploadService,
            HospitalManagementDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email address")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First name(s)")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of birth")]
            public DateTime DateOfBirth{ get; set; }

            [Required]
            [Display(Name = "Profile picture")]
            public IFormFile ProfilePicture { get; set; }

            [Required]
            [Display(Name = "Identity number")]
            public string IdNumber { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Display(Name = "Phone number")]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Alternate phone number")]
            [Phone(ErrorMessage = "Invalid alternate phone number format.")]
            public string? AlternatePhoneNumber { get; set; }

            [StringLength(100, ErrorMessage = "Street name cannot exceed 100 characters.")]
            public string? Street { get; set; }

            [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
            public string? City { get; set; }


            [Required(ErrorMessage = "Province is required.")]
            [Display(Name = "Province")]
            public Province Province { get; set; }

            [Display(Name = "Blood type")]
            public BloodType? BloodType { get; set; }

            [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
            public string? PostalCode { get; set; }

            [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
            public string? Country { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var newPatient = new Patient
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    AlternatePhoneNumber = Input.AlternatePhoneNumber,
                    Address = string.Join(", ", new[] {
                              Input.Street, Input.City, Input.Province.ToString(), Input.PostalCode, Input.Country
                                                       }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    IsActive = true,
                    AccessFailedCount = 0,
                    BloodType = Input.BloodType,
                    DateOfBirth = Input.DateOfBirth,
                    CreatedDateTime = DateTime.Now,
                    IdNumber = Input.IdNumber,
                    Gender = Input.Gender,
                    IsSuspended = false,
                    IsFirstTimeLogin = false,
                    IsDeleted = false
                };

                if (Input.ProfilePicture != null && Input.ProfilePicture.Length > 0)
                {
                    var playerProfilePicturePath = await _fileUploadService.UploadFileAsync(Input.ProfilePicture);
                    newPatient.ProfilePicture = playerProfilePicturePath;
                }

                await _userStore.SetUserNameAsync(newPatient, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(newPatient, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(newPatient, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(newPatient);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newPatient);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    var medicalHistory = new PatientMedicalHistory
                    {
                        PatientId = newPatient.Id,
                        AccessCode = null,
                        MedicalHistories = null,
                        QrCodeImage = null,
                        CreatedAt = DateTime.Now,
                        ExpiresAt = DateTime.Now,
                    };

                    _context.Add(medicalHistory);
                    await _context.SaveChangesAsync();  

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(newPatient, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
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
