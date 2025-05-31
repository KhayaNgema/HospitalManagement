using System.Diagnostics;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly HospitalManagementDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            UserManager<UserBaseModel> userManager,
            HospitalManagementDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string tab)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var roles = await _userManager.GetRolesAsync(user);

                if (user.IsFirstTimeLogin && roles.Any())
                {
                    user.IsFirstTimeLogin = false;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    return Redirect("/Identity/Account/Manage/ChangeFirstTimeLoginPassword");
                }
                else
                {
                    ViewBag.ActiveTab = string.IsNullOrEmpty(tab) ? "sportnews" : tab;
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                ViewBag.ActiveTab = string.IsNullOrEmpty(tab) ? "sportnews" : tab;
                return View();
            }
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.LoggedInUser = $"{user.FirstName} {user.LastName}";

            if (roles.Contains("System Administrator"))
            {
                return View("SystemAdministratorDashboard");
            }
            else if (roles.Contains("Doctor"))
            {
                return View("DoctorDashboard");
            }
            else if (roles.Contains("Paramedic"))
            {
                return View("ParamedicDashboard");
            }
            else if (roles.Contains("Kitchen Staff"))
            {
                return View("KitchenStaffDashboard");
            }
            else
            {
                return View("PatientDashboard");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new  { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
