using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;

namespace HospitalManagement.Controllers
{
    public class CartsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HospitalManagementDbContext _context;

        public CartsController(IHttpContextAccessor httpContextAccessor, HospitalManagementDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var cartJson = session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson)
                ? new Cart()
                : JsonSerializer.Deserialize<Cart>(cartJson) ?? new Cart();
        }

        private void SaveCart(Cart cart)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public IActionResult ShoppingCart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId, int quantity)
        {
            var menuItem = await _context.MenuItems.FindAsync(itemId);

            if (menuItem == null)
            {
                return NotFound("Menu item not found.");
            }

            var cart = GetCart();
            cart.AddItem(menuItem, quantity);
            SaveCart(cart);

            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult RemoveItem(int itemId)
        {
            var cart = GetCart();
            cart.RemoveItem(itemId, _httpContextAccessor);
            SaveCart(cart);

            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int itemId, int quantity)
        {
            var cart = GetCart();
            cart.UpdateQuantity(itemId, quantity);
            SaveCart(cart);

            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            var cart = new Cart();
            SaveCart(cart);

            return RedirectToAction("ShoppingCart");
        }
    }
}
