using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class MedicationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
