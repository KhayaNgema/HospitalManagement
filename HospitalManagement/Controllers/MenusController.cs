using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class MenusController : Controller
    {
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly FileUploadService _fileUploadService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RandomPasswordGeneratorService _passwordGenerator;
        private readonly IEmailSender _emailSender;
        private readonly EmailService _emailService;
        private readonly HospitalManagementDbContext _context;
        private readonly IActivityLogger _activityLogger;
        private readonly OrderCalculationService _orderCalculationService;

        public MenusController(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            SignInManager<UserBaseModel> signInManager,
            IEmailSender emailSender,
            FileUploadService fileUploadService,
            RoleManager<IdentityRole> roleManager,
            RandomPasswordGeneratorService passwordGenerator,
            EmailService emailService,
            OrderCalculationService orderCalculationService,
            HospitalManagementDbContext db,
            IActivityLogger activityLogger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _roleManager = roleManager;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _context = db;
            _activityLogger = activityLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            var user = await _userManager.GetUserAsync(User);

            var categories = await _context.Categories
                .ToListAsync();

            ViewBag.Categories = categories;

            var cartItems = _context.CartItems
                                .Where(ci => ci.Patient.Id == user.Id && !ci.Deleted)
                                .ToList();

            var menuItems = await _context.MenuItems
                .ToListAsync();

            /*            decimal totalPrice = _orderCalculationService.CalculateTotalPrice(cartItems);*/

            /*            ViewBag.TotalPrice = totalPrice;
            */
            ViewBag.Categories = categories;
            var viewModel = new MenuViewModel
            {
                MenuItems = menuItems,
                CartItems = cartItems
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MenuItems()
        {
            var menuItems = await _context.MenuItems
                .ToListAsync();

            var categories = await _context.Categories
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(menuItems);
        }

        [Authorize(Roles = "Kitchen Staff")]
        [HttpGet]
        public async Task<IActionResult> NewItem()
        {
            var categories = await _context.Categories
                .ToListAsync();

            ViewBag.Categories = categories;


            return View();
        }

        [Authorize(Roles = "Kitchen Staff")]
        [HttpPost]
        public async Task<IActionResult> NewItem(MenuItemViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var menuItem = new MenuItem
                {
                    ItemName = viewModel.ItemName,
                    CategoryId = viewModel.CategoryId,
                    ItemDescription = viewModel.ItemDescription,
                    Price = viewModel.Price,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedById = user.Id,
                    IsSelected = false,
                };

                if (viewModel.ItemImages != null && viewModel.ItemImages.Length > 0)
                {
                    var playerProfilePicturePath = await _fileUploadService.UploadFileAsync(viewModel.ItemImages);
                    menuItem.ItemImage = playerProfilePicturePath;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added {viewModel.ItemName} as your new hospital menu items";

                return RedirectToAction(nameof(MenuItems));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add new menu item: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }


            var categories = await _context.Categories
                .ToListAsync();

            ViewBag.Categories = categories;


            return View();
        }
    }
}
