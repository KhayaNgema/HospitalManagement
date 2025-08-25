using Cafeteria.Services;
using Hangfire;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

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
        private readonly IEncryptionService _encryptionService;
        private readonly ReceiveMedicationOrder _receiveMedicationOrder;

        public OrdersController(
            OrderNumberGenerator orderNumberGenerator,
            HospitalManagementDbContext context,
            OrderCalculationService orderCalculationService,
            IPaymentService paymentService,
            DeviceInfoService deviceInfoService,
            UserManager<UserBaseModel> userManager,
            SignInManager<UserBaseModel> signInManager,
            CartService cartService,
            IEncryptionService encryptionService,
            ReceiveMedicationOrder receiveMedicationOrder)
        {
            _cartService = cartService;
            _orderNumberGenerator = orderNumberGenerator;
            _context = context;
            _orderCalculationService = orderCalculationService;
            _paymentService = paymentService;
            _deviceInfoService = deviceInfoService;
            _userManager = userManager;
            _signInManager = signInManager;
            _encryptionService = encryptionService;
            _receiveMedicationOrder = receiveMedicationOrder;
        }

        public IActionResult SuccessfullyOrdered()
        {
            return View();
        }

        [Authorize(Roles = "Supplier Driver")]
        [HttpGet]
        public async Task<IActionResult> PendingMedicationDeliveries()
        {
            var deliveries = await _context.MedicationOrders
                .Where(d => d.Status == OrderStatus.Packaged ||
                d.Status == OrderStatus.OnTheWay)
                .Include(d=> d.Pharmacist)
                .Include(d => d.OrderItems)
                   .ThenInclude(d => d.MedicationStock)
                     .ThenInclude(d => d.Medication )
                .ToListAsync();

            return View(deliveries);
        }

        [Authorize(Roles = "Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> PackageMedication(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return NotFound();

            var decryptedOrderId = _encryptionService.DecryptToInt(orderId);

            var order = await _context.MedicationOrders
                .Where(o => o.OrderId == decryptedOrderId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MedicationStock)
                        .ThenInclude(ms => ms.Medication)
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return View(order);
        }


        [Authorize(Roles = "Pharmacist, Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> MedicationOrderDetails(string orderId)
        {
            var decryptedOrderId = _encryptionService.DecryptToInt(orderId);

            var order = await _context.MedicationOrders
                .Where(o => o.OrderId == decryptedOrderId)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MedicationStock)
                .ThenInclude(o => o.Medication)
                .ToListAsync();

            return View(order);
        }


        [Authorize(Roles = "Pharmacist, Supplier Administrator")]
        [HttpGet]
        public async Task<IActionResult> MedicationOrders()
        {
            var orders = await _context.MedicationOrders
                .Include(o => o.OrderItems)
                .Include(o => o.Pharmacist)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Orders()
        {
            try
            {
                var allOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .Where(o => o.Status == OrderStatus.Collected)
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
                .ThenInclude(o => o.MenuItem)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> PreparedOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Ready_For_Delivery)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .ToListAsync();


            foreach (var order in orders)
            {
                var patient = await _context.Patients
                    .Where(p => p.Id == order.PatientId)
                    .FirstOrDefaultAsync();

                var admission = await _context.Admissions
                    .Where(a => a.PatientId == patient.Id &&
                    a.PatientStatus == PatientStatus.Admitted)
                    .FirstOrDefaultAsync();

                ViewBag.Department = admission.Department;
                ViewBag.RoomNo = admission.RoomNumber;
                ViewBag.BedNo = admission.BedNumber;
                ViewBag.PhoneNo = patient.PhoneNumber;
                ViewBag.Email = patient.Email;
            }

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewOrderIdsForCollection()
        {
            try
            {
                var newOrderIds = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Ready_For_Delivery)
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


        [HttpGet]
        public IActionResult GetPackagedOrderItems(int orderId)
        {
            var packagedItemIds = _context.MedicationOrderItems
                .Where(oi => oi.OrderId == orderId && oi.IsPackaged)
                .Select(oi => oi.OrderItemId)
                .ToList();

            return Json(packagedItemIds);
        }


        [Authorize(Roles = "Pharmacist")]
        [HttpGet]
        public async Task<IActionResult> ReceiveOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Missing orderId parameter");
            }

            string decodedOrderId = WebUtility.UrlDecode(orderId);

            int decryptedOrderId;

            try
            {
                decryptedOrderId = _encryptionService.DecryptToInt(decodedOrderId);
            }
            catch
            {
                return BadRequest("Invalid orderId");
            }

            var order = await _context.MedicationOrders
                .Where(o => o.OrderId == decryptedOrderId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MedicationStock)
                        .ThenInclude(ms => ms.Medication)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> ReceiveOrder(int orderId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var order = await _context.MedicationOrders
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();

                order.Status = OrderStatus.Collected;
                order.LastUpdated = DateTime.Now;
                order.ReceivedById = user.Id;

                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully received your order. Thanks for verifying qantities.";

                return RedirectToAction(nameof(MedicationOrders));
            }
            catch
            {

            }

            return View();
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            var order = await _context.MedicationOrders.FindAsync(orderId);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.OnTheWay;
            await _context.SaveChangesAsync();

            var encryptedOrderId = _encryptionService.Encrypt(order.OrderId);

            BackgroundJob.Enqueue(() => _receiveMedicationOrder.NotifyPharmacistOrderOnTheWayAsync(encryptedOrderId));
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var order = await _context.MedicationOrders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Collected;
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PackageOrderItem(int orderItemId)
        {
            var item = _context.MedicationOrderItems.Find(orderItemId);
            if (item == null)
                return NotFound();

            if (!item.IsPackaged)
            {
                item.IsPackaged = true;
                _context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CompletePackaging(int orderId)
        {

            try
            {
                var encryptedOrderId = _encryptionService.Encrypt(orderId);

                var order = await _context.MedicationOrders
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();

                if(order.Status == OrderStatus.Packaged)
                {
                    TempData["Message"] = $"This order has been already packaged and waiting for delivery.";

                    return RedirectToAction(nameof(PackageMedication), new {orderId = encryptedOrderId});
                }

                order.Status = OrderStatus.Packaged;

                _context.Update(order);

                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully packaged an order with order number: {order.OrderNumber}. The order has been transferred to the available driver and ready to be delivered.";

                return RedirectToAction(nameof(MedicationOrders));
            }
            catch
            {
                var encryptedOrderId = _encryptionService.Encrypt(orderId);

                return View("PackageMedication", new { orderId = encryptedOrderId });

            }
        }


        [HttpGet]
        public async Task<IActionResult> NewMedicationOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.MedicationCartItems
                .Where(ci => ci.MedicationCart.UserId == user.Id && !ci.Deleted)
                .Include(ci => ci.MedicationStock)
                    .ThenInclude(ms => ms.Medication)
                 .Include(ci => ci.MedicationStock)
                    .ThenInclude(ms => ms.Supplier)
                .ToListAsync();

            cartItems = cartItems.Where(ci => ci.MedicationStock != null).ToList();

            var viewModel = new MedicationOrderViewModel
            {
                MedicationCartItems = cartItems
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> NewMedicationOrder(MedicationOrderViewModel viewModel)
        {
            try
            {
                if (viewModel == null)
                    return Json(new { success = false, message = "Invalid order data." });

                if (viewModel.MedicationCartItems == null || !viewModel.MedicationCartItems.Any())
                    return Json(new { success = false, message = "Cart is empty." });

                var cartItems = await GetMedicationCartItemsForCurrentUserAsync();

                var user = await _userManager.GetUserAsync(User);

                if (string.IsNullOrEmpty(user.Id))
                    return Json(new { success = false, message = "User not authenticated." });

                var deviceInfo = await _deviceInfoService.GetDeviceInfo();
                if (deviceInfo == null)
                    return Json(new { success = false, message = "Device information not available." });

                var order = new MedicationOrder
                {
                    OrderNumber = _orderNumberGenerator.GenerateMedicationOrderNumber(),
                    OrderDate = DateTime.Now,
                    PharmacistId = user.Id,
                    LastUpdated = DateTime.Now,
                    OrderItems = cartItems.Select(ci => new MedicationOrderItem
                    {
                        StockId = ci.StockId,
                        Quantity = ci.Quantity,
                        IsPackaged = false                }).ToList(),
                    Status = OrderStatus.Pending,
                   
                };

                _context.Add(order);
                _context.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SuccessfullyOrdered));
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

        public async Task<List<MedicationCartItem>> GetMedicationCartItemsForCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var cartItems = _context.MedicationCartItems
                .Where(item => item.PharmacistId == user.Id)
                .Include(item => item.MedicationStock)
                .ThenInclude(item => item.Medication)
                .Include(ci => ci.MedicationStock)
                    .ThenInclude(ms => ms.Supplier)
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

                order.Status = OrderStatus.Ready_For_Delivery;
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

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Failed to update order status. See logs for details." });
            }
        }
    }

}

