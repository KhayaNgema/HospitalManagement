using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
