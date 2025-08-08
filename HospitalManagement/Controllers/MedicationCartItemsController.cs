using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagement.Controllers
{
    public class MedicationCartItemsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly OrderCalculationService _orderCalculationService;
        private readonly ILogger<MedicationCartItemsController> _logger;
        private readonly UserManager<UserBaseModel> _userManager;

        public MedicationCartItemsController(
            HospitalManagementDbContext context,
            OrderCalculationService orderCalculationService,
            ILogger<MedicationCartItemsController> logger,
            UserManager<UserBaseModel> userManager)
        {
            _context = context;
            _orderCalculationService = orderCalculationService;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> CreateCartItemForm(int menuItemId)
        {
            var menuItem = await _context.MedicationStocks
                .Include(m => m.Medication)
                .FirstOrDefaultAsync(m => m.StockId == menuItemId);

            if (menuItem != null)
            {
                var viewModel = new MedicationCartItemViewModel
                {
                    StockId = menuItem.StockId,
                    MedicationName = menuItem.Medication.MedicationName,
                    Quantity = menuItem.Quantity
                };
                return PartialView("_CreateMedicationCartItemForm", viewModel);
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cartItem = await _context.MedicationCartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound();
                }

                cartItem.Deleted = true;
                _context.MedicationCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart item");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var cartItem = await _context.MedicationCartItems.FindAsync(id);

                if (cartItem == null)
                {
                    return NotFound();
                }

                cartItem.Quantity = quantity;

                _context.Update(cartItem);

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                return Json(new { success = false, errorMessage = "An error occurred while updating quantity." });
            }
        }

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddCartItem(MedicationCartItemViewModel viewModel)
{
    try
    {
        var user = await _userManager.GetUserAsync(User);

        // Find existing cart or create new one if missing
        var cart = await _context.MedicationCarts
            .Where(c => c.UserId == user.Id)
            .FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new MedicationCart
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.MedicationCarts.Add(cart);
            await _context.SaveChangesAsync();  // Save to get CartId generated
            _logger.LogInformation("Created new cart for user {UserId} with CartId {CartId}", user.Id, cart.CartId);
        }

        // Check if the item already exists in the cart
        var existingCartItem = await _context.MedicationCartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId
                                    && ci.StockId == viewModel.StockId
                                    && !ci.Deleted);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += viewModel.Quantity;
            _context.MedicationCartItems.Update(existingCartItem);
            _logger.LogInformation("Updated cart item: StockId={StockId}, NewQuantity={Quantity}",
                existingCartItem.StockId, existingCartItem.Quantity);
        }
        else
        {
            var newCartItem = new MedicationCartItem
            {
                PharmacistId = user.Id,
                CartId = cart.CartId,
                StockId = viewModel.StockId,
                Quantity = viewModel.Quantity,
                Deleted = false
            };
            _context.MedicationCartItems.Add(newCartItem);
            _logger.LogInformation("Added new cart item: StockId={StockId}, Quantity={Quantity}",
                newCartItem.StockId, newCartItem.Quantity);
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create cart item");
        return Json(new
        {
            success = false,
            message = "Failed to create cart item: " + ex.Message,
            errorDetails = new
            {
                InnerException = ex.InnerException?.Message,
                StackTrace = ex.StackTrace
            }
        });
    }
}




        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartId = await _context.MedicationCarts
                .Where(c => c.UserId == userId)
                .Select(c => c.CartId)
                .FirstOrDefaultAsync();

            if (cartId == 0)
                return Json(0);

            var count = await _context.MedicationCartItems
                .Where(ci => ci.CartId == cartId && !ci.Deleted)
                .Select(ci => ci.StockId)
                .Distinct()
                .CountAsync();

            return Json(count);
        }


        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var cartItems = await _context.MedicationCartItems
                    .Include(c => c.MedicationStock)
                        .ThenInclude(c => c.Medication)
                    .Include(ci => ci.MedicationStock)
                        .ThenInclude(ms => ms.Supplier)
                    .Include(c => c.Pharmacist)
                    .Where(ci => ci.PharmacistId == user.Id && !ci.Deleted)
                    .ToListAsync();

                return PartialView("_MedicationCartItemsPartial", cartItems);
            }
            catch (Exception ex)
            {
                return PartialView("_MedicationCartItemsPartial", new List<CartItem>());
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
