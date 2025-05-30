using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class MenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
