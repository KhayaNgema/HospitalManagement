using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using HospitalManagement.Data;
using Cafeteria.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Hangfire;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Cafeteria.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderNumberGenerator _orderNumberGenerator;
        private readonly CartService _cartService;
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly HospitalManagementDbContext _context;
        private readonly OrderCalculationService _orderCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly DeviceInfoService _deviceInfoService;

        public OrdersController(
            OrderNumberGenerator orderNumberGenerator,
            HospitalManagementDbContext context,
            OrderCalculationService orderCalculationService,
            IPaymentService paymentService,
            DeviceInfoService deviceInfoService,
            UserManager<UserBaseModel> userManager,
            SignInManager<UserBaseModel> signInManager,
            CartService cartService)
        {
            _cartService = cartService;
            _orderNumberGenerator = orderNumberGenerator;
            _context = context;
            _orderCalculationService = orderCalculationService;
            _paymentService = paymentService;
            _deviceInfoService = deviceInfoService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Orders()
        {
            try
            {
                var allOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

                var latestUpdatedOrders = allOrders
                    .GroupBy(o => o.OrderId)
                    .Select(g => g.OrderByDescending(o => o.LastUpdated).First())
                    .ToList();

                return View(latestUpdatedOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .Where(o => o.PatientId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> PendingOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude (o => o.MenuItem)  
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> PreparedOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Ready_For_Collection)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewOrderIdsForCollection()
        {
            try
            {
                var newOrderIds = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Ready_For_Collection)
                    .Select(o => o.OrderId)
                    .ToListAsync();

                return Json(new { success = true, orderIds = newOrderIds });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, error = "Error retrieving new order IDs." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOrdersByIds([FromBody] int[] orderIds)
        {
            try
            {
                var orders = _context.Orders
                    .Where(o => orderIds.Contains(o.OrderId))
                    .ToList();

                return Json(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching orders: " + ex.Message);
                return Json(new { success = false, error = "Error fetching orders." });
            }
        }

        public async Task<IActionResult> NewOrder()
        {
            var cartItems = await GetCartItemsForCurrentUserAsync();
            decimal totalPrice = _orderCalculationService.CalculateTotalPrice(cartItems);
            ViewBag.TotalPrice = totalPrice;

            var viewModel = new OrderViewModel
            {
                CartItems = cartItems
            };

            ViewBag.CartItems = cartItems;

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> NewOrder(OrderViewModel viewModel)
        {
            try
            {
                if (viewModel == null)
                    return Json(new { success = false, message = "Invalid order data." });

                if (viewModel.CartItems == null || !viewModel.CartItems.Any())
                    return Json(new { success = false, message = "Cart is empty." });

                var cartItems = await GetCartItemsForCurrentUserAsync();

                var totalPrice = _orderCalculationService.CalculateTotalPrice(cartItems);

                var user = await _userManager.GetUserAsync(User);

                if (string.IsNullOrEmpty(user.Id))
                    return Json(new { success = false, message = "User not authenticated." });

                var deviceInfo = await _deviceInfoService.GetDeviceInfo();
                if (deviceInfo == null)
                    return Json(new { success = false, message = "Device information not available." });

                var order = new Order
                {
                    OrderNumber = _orderNumberGenerator.GenerateOrderNumber(),
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    PatientId = user.Id,
                    LastUpdated = DateTime.Now,
                    OrderItems = cartItems.Select(ci => new OrderItem
                    {
                        MenuItemId = ci.MenuItemId,
                        MenuItemName = ci.MenuItem.ItemName,
                        Quantity = ci.Quantity,
                        SubTotal = ci.Subtotal,
                        MenuItemPrice = ci.MenuItem.Price
                    }).ToList(),
                    IsPaid = true,
                    Status = OrderStatus.Pending
                };

                _context.Add(order);
                _context.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                return RedirectToAction("OrderPlacedSuccessfully", new
                {
                    orderNumber = order.OrderNumber,
                    orderStatus = order.Status.ToString(),
                    amountPaid = order.TotalPrice,
                    orderId = order.OrderId,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to redirect to payfast: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }

        [Authorize]
        public async Task<IActionResult> ProofOfPurchase(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.PatientId == user.Id);

            if (order == null)
                return NotFound();

            return PartialView("_ProofOfPurchasePartial", order);
        }


        public async Task<IActionResult> OrderPlacedSuccessfully(string orderNumber, string orderStatus, decimal amountPaid, int orderId)
        {
            ViewBag.AmountPaid = amountPaid;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.OrderNumber = orderNumber;
            ViewBag.OrderId = orderId;

            return View();
        }

        public async Task<List<CartItem>> GetCartItemsForCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var cartItems = _context.CartItems
                .Where(item => item.PatientId == user.Id)
                .Include(item => item.MenuItem)
                .ToList();

            return cartItems;
        }


        private string GeneratePaymentReferenceNumber()
        {
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString("D2");
            var day = DateTime.Now.Day.ToString("D2");

            const string numbers = "0123456789";
            const string fineLetters = "MO";

            var random = new Random();
            var randomNumbers = new string(Enumerable.Repeat(numbers, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{year}{month}{day}{randomNumbers}{fineLetters}";
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MoveToReadyForCollection(int orderId)
        {
            try
            {

                var order = _context.Orders
                               .Include(o => o.OrderItems)
                               .SingleOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    Debug.WriteLine("Order not found.");
                    return Json(new { success = false, message = "Order not found." });
                }

                Debug.WriteLine("Order found: " + order.OrderNumber);

                order.Status = OrderStatus.Ready_For_Collection;
                order.LastUpdated = DateTime.Now;
                _context.SaveChanges();

                Debug.WriteLine("Order status updated to Ready For Collection.");

                return Json(new { success = true, message = "Order status updated to Ready For Collection." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Failed to update order status. See logs for details." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MoveToCollected(int orderId)
        {
            try
            {
                var order = await _context.Orders
                               .Include(o => o.OrderItems)
                               .SingleOrDefaultAsync(o => o.OrderId == orderId);

                order.Status = OrderStatus.Collected;
                order.LastUpdated = DateTime.Now;
                _context.SaveChanges();

                var patientBill = await _context.PatientBills
                    .Where(pb => pb.PatientId == order.PatientId)
                    .Include(pb => pb.Services)
                    .Include(pb => pb.Patient)
                    .FirstOrDefaultAsync();

                var admission = await _context.Admissions
                    .Include(a => a.Booking)
                    .Where(a => a.PatientId == order.PatientId &&
                    a.PatientStatus == PatientStatus.Admitted)
                    .FirstOrDefaultAsync();

                foreach (var orderItem in order.OrderItems)
                {
                    var menuItem = _context.MenuItems
                        .FirstOrDefault(m => m.ItemId == orderItem.MenuItemId);

                    if (menuItem != null)
                    {
                        string shortCode = new string(menuItem.ItemName
                            .Where(char.IsLetterOrDigit)
                            .Take(3)
                            .ToArray())
                            .ToUpper();

                        var newBillService = new PatientBillServices
                        {
                            AdmissionId = admission.AdmissionId,
                            BookingId = admission.BookingId,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            PatientBillId = patientBill.BillId,
                            ReferenceNumber = $"HO-WARD-FOO-{shortCode}",
                            ServiceName = menuItem.ItemName ?? "Ordered Meals",
                            ServiceType = "Meals",
                            Subtotal = menuItem.Price
                        };

                        _context.PatientBillServices.Add(newBillService);

                        patientBill.Services.Add(newBillService);
                        patientBill.PayableTotalAmount += newBillService.Subtotal;
                    }
                }


                _context.Update(patientBill);
                await _context.SaveChangesAsync();

                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Failed to update order status. See logs for details." });
            }
        }
    }

}

