using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;

        public CategoriesController(HospitalManagementDbContext context,
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


        [HttpGet]
        public async Task<IActionResult> MedicationCategories()
        {
            var categories = await _context.MedicationCategories
                .Include(c => c.CreatedBy)
                 .Include(c => c.ModifiedBy)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.CreatedBy)
                 .Include(c => c.ModifiedBy)
                .ToListAsync();

            return View(categories);
        }
        
        [HttpGet]
        public async Task<IActionResult> NewMedicationCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewMedicationCategory(MedicationCategory model)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var category = new MedicationCategory
                {
                    CategoryName = model.CategoryName,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedById = user.Id
                };

                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully created the {category.CategoryName} category.";

                return RedirectToAction(nameof(MedicationCategories));
            }
            catch (Exception ex) 
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to create new category: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }


            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> NewCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCategory(Category model)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var category = new Category
                {
                    CategoryName = model.CategoryName,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedById = user.Id
                };

                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully created the {category.CategoryName} category.";

                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to create new category: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }


            return View(model);
        }

    }
}
