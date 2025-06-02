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
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully placed your order.";

                return RedirectToAction(nameof(OrderPlacedSuccessfully));
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


        public async Task<IActionResult> PayFastReturn(int paymentId)
        {
            try
            {
                var cartItems = await GetCartItemsForCurrentUserAsync();

                var totalPrice = _orderCalculationService.CalculateTotalPrice(cartItems);

                var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == paymentId);

                if (payment == null || payment.AmountPaid != totalPrice || !_paymentService.ValidatePayment(payment))
                {
                    return Json(new { success = false, message = "Invalid or failed payment." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var order = new Order
                {
                    OrderNumber = _orderNumberGenerator.GenerateOrderNumber(),
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    PatientId = userId,
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

                /*                var invoice = new Invoice
                                {
                                    OrderId = order.OrderId,
                                    PaymentId = payment.PaymentId,
                                    UserId = userId,
                                    InvoiceTimeStamp = DateTime.Now,
                                    Items = order.OrderItems.ToList(),
                                    TotalAmount = order.TotalPrice
                                };

                                _db.Invoices.Add(invoice);*/
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderPlacedSuccessfully", new
                {
                    orderNumber = order.OrderNumber,
                    orderStatus = order.Status.ToString(),
                    amountPaid = order.TotalPrice
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Failed to process payment: " + ex.Message });
            }
        }


        public async Task<IActionResult> PayFastReturn(int paymentId, string encryptedTransferId, decimal totalPrice)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Payment with PaymentId: {paymentId} not found.");
                    return Json(new { success = false, message = "Payment not found." });
                }

                decimal roundedAmountPaid = Math.Round(payment.AmountPaid, 2);
                decimal roundedTotalPrice = Math.Round(totalPrice, 2);

                if (Math.Abs(roundedAmountPaid - roundedTotalPrice) > 0.01m)
                {
                    System.Diagnostics.Debug.WriteLine($"Amount mismatch: Payment AmountPaid = {roundedAmountPaid}, totalPrice = {roundedTotalPrice}");
                    return Json(new { success = false, message = $"Invalid payment amount. AmountPaid: {roundedAmountPaid}, totalPrice: {roundedTotalPrice}" });
                }

                if (!_paymentService.ValidatePayment(payment))
                {
                    return Json(new { success = false, message = "Payment validation failed." });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not authenticated.");
                    return Json(new { success = false, message = "User not authenticated." });
                }

                payment.Status = PaymentPaymentStatus.Successful;
                payment.AmountPaid = roundedAmountPaid;


                _context.Update(payment);
                await _context.SaveChangesAsync();

/*                var newInvoice = new Invoice
                {
                    PaymentId = payment.PaymentId,
                    TransferId = playerTransfer.TransferId,
                    InvoiceTimeStamp = DateTime.Now,
                    CreatedById = user.Id,
                    InvoiceNumber = GenerateInvoiceNumber(paymentId),
                    IsEmailed = true,
                    Transfer = playerTransfer
                };*/

                _context.Update(payment);
                await _context.SaveChangesAsync();

            /*    string emailBody = $@"
            <p>Dear {playerTransfer.CustomerClub.ClubName} Management,</p>
            <p>We are pleased to inform you that the player transfer of {playerTransfer.Player.FirstName} {playerTransfer.Player.LastName} has been successfully completed.</p>
            <p>The proof of this transfer is available on your club's portal.</p>
            <p>Transfer Details:</p>
            <ul>
                <li><strong>Player:</strong> {playerTransfer.Player.FirstName} {playerTransfer.Player.LastName}</li>
                <li><strong>Transfer Amount:</strong> {payment.AmountPaid:C}</li>
                <li><strong>From Club:</strong> {playerTransfer.SellerClub.ClubName}</li>
                <li><strong>To Club:</strong> {playerTransfer.CustomerClub.ClubName}</li>
            </ul>
            <p>If you have any questions, feel free to contact us.</p>
            <p>Best regards,</p>
            <p>Diski 360 Team</p>
        ";


                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email, "Proof of Player Transfer", emailBody, "Diski 360"));*/
                TempData["Message"] = $"You have successfully cleared your  bills.";

                return RedirectToAction("MyBills", "Billings");
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to process payment: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }
        }

        public async Task<IActionResult> OrderPlacedSuccessfully(string orderNumber, string orderStatus, decimal amountPaid)
        {
            ViewBag.AmountPaid = amountPaid;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.OrderNumber = orderNumber;

            return View();
        }

        private string GeneratePayFastPaymentUrl(int paymentId, decimal amount, string returnUrl, string cancelUrl)
        {
            string merchantId = "10033052";
            string merchantKey = "708c7udni72oo";
            string amountString = amount.ToString().Replace(',', '.');

            return $"https://sandbox.payfast.co.za/eng/process?merchant_id={merchantId}&merchant_key={merchantKey}&return_url={returnUrl}&cancel_url={cancelUrl}&amount={amountString}&item_name=Order+Payment&payment_id={paymentId}";
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
        public JsonResult MoveToCollected(int orderId)
        {
            try
            {
                Debug.WriteLine("MoveToCollected action method started...");
                Debug.WriteLine("Received orderId: " + orderId);

                var order = _context.Orders
                               .Include(o => o.OrderItems)
                               .SingleOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    Debug.WriteLine("Order not found.");
                    return Json(new { success = false, message = "Order not found." });
                }

                Debug.WriteLine("Order found: " + order.OrderNumber);

                order.Status = OrderStatus.Collected;
                order.LastUpdated = DateTime.Now;
                _context.SaveChanges();

                Debug.WriteLine("Order status updated to Collected.");

                return Json(new { success = true, message = "Order status updated to Collected." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Failed to update order status. See logs for details." });
            }
        }
    }

}

