
using HospitalManagement.Data;
using HospitalManagement.Helpers;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace HospitalManagementSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(HospitalManagementDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int menuItemId, int quantity)
        {
            var menuItem = _context.MenuItems.FirstOrDefault(m => m.ItemId == menuItemId);
            if (menuItem != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = GetCart();

                var existingCartItem = cart.Items.FirstOrDefault(ci => ci.MenuItemId == menuItemId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        PatientId = userId,
                        MenuItemId = menuItemId,
                        MenuItem = menuItem,
                        Quantity = quantity
                    };
                    cart.Items.Add(newCartItem);
                }

                SaveCart(cart);

                return Json(new
                {
                    cartItemCount = cart.Items.Count,
                    total = cart.CalculateTotal()
                });
            }

            return Json(new { error = "Failed to add item to cart." });
        }

        public IActionResult GetCartItems()
        {
            try
            {
                var cartItems = GetCart().Items;
                var html = RenderCartItemsHtml(cartItems);
                return Content(html, "text/html");
            }
            catch (System.Exception ex)
            {
                return Content("An error occurred while fetching cart items: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            try
            {
                var cart = GetCart();
                cart.UpdateQuantity(menuItemId, quantity);
                SaveCart(cart);
                return Json(new { success = true });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int menuItemId)
        {
            try
            {
                var cart = GetCart();
                cart.RemoveItem(menuItemId, _httpContextAccessor);
                SaveCart(cart);
                return Json(new { success = true, cartItemCount = cart.Items.Count });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            var cart = new Cart();
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        private Cart GetCart()
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");
            if (cart == null)
            {
                cart = new Cart();
                SaveCart(cart);
            }
            return cart;
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetObject("Cart", cart);
        }

        private string RenderCartItemsHtml(List<CartItem> cartItems)
        {
            var htmlBuilder = new StringBuilder();

            if (cartItems != null && cartItems.Any())
            {
                foreach (var cartItem in cartItems)
                {
                    var menuItem = _context.MenuItems.FirstOrDefault(m => m.ItemId == cartItem.MenuItemId);
                    if (menuItem != null)
                    {
                        htmlBuilder.Append("<tr>");
                        htmlBuilder.Append($"<td>{menuItem.ItemName}</td>");
                        htmlBuilder.Append($"<td><input type='number' class='quantity-input form-control form-control-sm' value='{cartItem.Quantity}' min='1' data-cartitemid='{cartItem.CartItemId}'></td>");
                        htmlBuilder.Append($"<td>R {cartItem.Subtotal}</td>");
                        htmlBuilder.Append($"<td class='actions'><button class='delete-item-btn btn btn-danger' data-itemid='{cartItem.CartItemId}'><i class='fas fa-trash-alt'></i></button></td>");
                        htmlBuilder.Append("</tr>");
                    }
                }
            }
            else
            {
                htmlBuilder.Append("<tr><td colspan='4'>Your cart is empty</td></tr>");
            }

            return htmlBuilder.ToString();
        }
    }
}
