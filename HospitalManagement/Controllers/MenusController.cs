using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class MenusController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public MenusController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            QrCodeService qrCodeService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;

        }
        public async Task<IActionResult> MenuItems()
        {
            var menuItems = await _context.MenuItems
                .ToListAsync();

            var categories = await _context.Categories
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(menuItems);
        }
    }
}
