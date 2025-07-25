﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;

namespace HospitalManagement.Areas.Identity.Pages.Account.Manage
{
    public class ChangeFirstTimeLoginPasswordModel : PageModel
    {
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly ILogger<ChangeFirstTimeLoginPasswordModel> _logger;
        private readonly HospitalManagementDbContext _context;
        private readonly EmailService _emailService;
        private readonly IActivityLogger _activityLogger;

        public ChangeFirstTimeLoginPasswordModel(
            UserManager<UserBaseModel> userManager,
            SignInManager<UserBaseModel> signInManager,
            ILogger<ChangeFirstTimeLoginPasswordModel> logger,
            HospitalManagementDbContext context,
            EmailService emailService,
            IActivityLogger activityLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _activityLogger = activityLogger;
        }

        /// <summary>
        ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Old temporal password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the (link unavailable) Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            string emailBody = $@"
                    Hi {user.FirstName} {user.LastName},<br/><br/>
                    Your temporal password has been changed successfully.  
                    Please note that it has expired and you will be required to use your new password for aunthetication.<br/><br/>
                    If you did not request this change, please contact our support team immediately.<br/><br>
                    Kind regards,<br/>
                    MediConnect Team
            ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Temporal Password Changed Successful",
                emailBody, "MediConnect");

            await _activityLogger.Log($"Changed temporal password at first time login", user.Id);

            return RedirectToPage("/Account/Login");
        }
    }
}