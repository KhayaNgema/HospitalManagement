using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly FileUploadService _fileUploadService;

        public SuppliersController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            QrCodeService qrCodeService,
            EmailService emailService,
            FileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _fileUploadService = fileUploadService;
        }

        [Authorize(Roles = "Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> Orders()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StockCategories()
        {
            var categories = await _context.StockCategories
                .ToListAsync();

            return View(categories);
        }

        [Authorize(Roles = "Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> MedicationStock()
        {
            var stockItems = await _context.MedicationStocks
                .Include(m => m.StockCategory)
                .Include(m => m.Medication)
                .ToListAsync();

            ViewBag.StockCategories = await _context.StockCategories.ToListAsync();

            return View(stockItems);
        }


        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> StockMenu()
        {
            var user = await _userManager.GetUserAsync(User);

            var stockItems = await _context.MedicationStocks
                .Include(m => m.StockCategory)
                .Include(m => m.Medication)
                .ToListAsync();

            ViewBag.Categories = await _context.StockCategories.ToListAsync();

            var cartItems = _context.MedicationCartItems
                                .Where(ci => ci.Pharmacist.Id == user.Id && !ci.Deleted)
                                .ToList();

            var viewModel = new StockMenuViewModel
            {
                MedicationStockItems = stockItems,
                MedicationCartItems = cartItems
            };

            return View(viewModel);
        }


        [Authorize(Roles = "System Administrator")]
        [HttpGet]
        public async Task<IActionResult> SupplierAdministrators(string supplierId)
        {
            var decryptedSupplierId = _encryptionService.DecryptToInt(supplierId);

            var admins = await _context.SupplierAdministrators
                .Where(a => a.SupplierId == decryptedSupplierId)
                .ToListAsync();

            ViewBag.SupplierId = supplierId;

            return View(admins);
        }



        [Authorize(Roles = "System Administrator, Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> SupplierDrivers(string supplierId)
        {
            var drivers = await _context.SupplierDrivers
                .ToListAsync();

            return View(drivers);
        }

        [Authorize(Roles = "System Administrator")]
        [HttpGet]
        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .ToListAsync();

            return View(suppliers);
        }


        [HttpGet]
        public async Task<IActionResult> NewStockCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewStockCategory(StockCategory model)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var category = new StockCategory
                {
                    CategoryName = model.CategoryName
                };

                _context.Add(category);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully created the {category.CategoryName} category.";

                return RedirectToAction(nameof(StockCategories));
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

        [Authorize(Roles = "Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> NewStock()
        {
            var user = await _userManager.GetUserAsync(User);

            var supplierId = (user as SupplierAdministrator).SupplierId;

            ViewBag.Medications = await _context.Medications.ToListAsync();
            ViewBag.StockCategories = await _context.StockCategories.ToListAsync();

            var viewModel = new NewStockViewModel
            {
                SupplierId = supplierId
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Supplier Administrator")]
        [HttpPost]
        public async Task<IActionResult> NewStock(NewStockViewModel viewModel)
        {
            ViewBag.Medications = await _context.Medications.ToListAsync();
            ViewBag.StockCategories = await _context.StockCategories.ToListAsync();

            try
            {
                var batchNumber = await GenerateBatchNumber();

                var newStock = new MedicationStock
                {
                    SupplierId = viewModel.SupplierId,
                    MedicationId = viewModel.MedicationId,
                    StockCategoryId = viewModel.StockCategoryId,
                    Quantity = viewModel.Quantity,
                    BatchNumber = batchNumber
                };

                _context.MedicationStocks.Add(newStock);
                await _context.SaveChangesAsync();

                newStock.QrCodeImage = _qrCodeService.GenerateQrCode(batchNumber);
                _context.Update(newStock);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully created a new stock.";

                return RedirectToAction(nameof(MedicationStock));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to stock: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View(viewModel);

        }

        private async Task<string> GenerateBatchNumber()
        {

            const string numbers = "0123456789";
            const string fineLetters = "MED-";

            var random = new Random();
            var randomNumbers = new string(Enumerable.Repeat(numbers, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{fineLetters}{randomNumbers}";
        }

        [Authorize(Roles = "System Administrator")]
        [HttpGet]
        public async Task<IActionResult> NewSupplier()
        {
            return View();
        }

        [Authorize(Roles = "System Administrator")]
        [HttpPost]
        public async Task<IActionResult> NewSupplier(NewSupplierViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var supplier = new Supplier
                {
                    SupplierName = viewModel.SupplierName,
                    DivisionDescription = viewModel.DivisionDescription,
                    Address = $"{viewModel.StreetLine1}, {viewModel.StreetLine2}, {viewModel.Suburb}" +
                    $"{viewModel.City}, {viewModel.Province}, {viewModel.PostalCode}, {viewModel.Country}",
                    CreatedById = user.Id,
                    CreatedDateTime = DateTime.Now,
                    ModifiedById = user.Id,
                    ModifiedDateTime = DateTime.Now,
                };


                if (viewModel.SupplierBadges != null && viewModel.SupplierBadges.Length > 0)
                {
                    var uploadedImagePath = await _fileUploadService.UploadFileAsync(viewModel.SupplierBadges);
                    supplier.SupplierBadge = uploadedImagePath;
                }

                _context.Add(supplier);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added {viewModel.SupplierName} as your new hospital supplier.";

                return RedirectToAction(nameof(Suppliers));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to create supplier: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View(viewModel);
        }
    }


}
