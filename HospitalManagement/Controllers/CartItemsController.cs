using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cafeteria.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly OrderCalculationService _orderCalculationService;
        private readonly ILogger<CartItemsController> _logger;
        private readonly UserManager<UserBaseModel> _userManager;

        public CartItemsController(
            HospitalManagementDbContext context,
            OrderCalculationService orderCalculationService,
            ILogger<CartItemsController> logger,
            UserManager<UserBaseModel> userManager)
        {
            _context = context;
            _orderCalculationService = orderCalculationService;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> CreateCartItemForm(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.ItemId == menuItemId);

            if (menuItem != null)
            {
                var viewModel = new CartItemViewModel
                {
                    MenuItemId = menuItem.ItemId,
                    MenuItemName = menuItem.ItemName,
                    MenuItemPrice = menuItem.Price
                };
                return PartialView("_CreateCartItemForm", viewModel);
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound();
                }

                cartItem.Deleted = true;
                _context.CartItems.Remove(cartItem);
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
        public async Task<IActionResult> UpdateQuantity(int id, int quantity, decimal price)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound();
                }

                cartItem.Quantity = quantity;
                cartItem.Subtotal = quantity * price;

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
        public async Task<IActionResult> AddCartItem(CartItemViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("_CreateCartItemForm", viewModel);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cartId = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .Select(c => c.CartId)
                    .FirstOrDefaultAsync();

                if (cartId == 0)
                {
                    return NotFound("Cart not found for the current user.");
                }

                var existingCartItem = await _context.CartItems
                    .Where(ci => ci.CartId == cartId && ci.MenuItemId == viewModel.MenuItemId && !ci.Deleted)
                    .FirstOrDefaultAsync();

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += viewModel.Quantity;
                    existingCartItem.Subtotal = existingCartItem.Quantity * viewModel.MenuItemPrice;

                    _context.CartItems.Update(existingCartItem);
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        PatientId = userId,
                        CartId = cartId,
                        MenuItemId = viewModel.MenuItemId,
                        Quantity = viewModel.Quantity,
                        Subtotal = viewModel.Quantity * viewModel.MenuItemPrice,
                        Deleted = false
                    };

                    _context.CartItems.Add(newCartItem);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating cart item");
                TempData["ErrorMessage"] = "Failed to add item to cart.";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartId = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => c.CartId)
                .FirstOrDefaultAsync();

            if (cartId == 0)
                return Json(0);

            var count = await _context.CartItems
                .Where(ci => ci.CartId == cartId && !ci.Deleted)
                .Select(ci => ci.MenuItemId)
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

                var cartItems = await _context.CartItems
                    .Include(c => c.MenuItem)
                    .Include(c => c.Patient)
                    .Where(ci => ci.PatientId == user.Id && !ci.Deleted)
                    .ToListAsync();

                decimal totalPrice = _orderCalculationService.CalculateTotalPrice(cartItems);

                ViewBag.TotalPrice = totalPrice;

                return PartialView("_CartItemsPartial", cartItems);
            }
            catch (Exception ex)
            {
                return PartialView("_CartItemsPartial", new List<CartItem>());
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
