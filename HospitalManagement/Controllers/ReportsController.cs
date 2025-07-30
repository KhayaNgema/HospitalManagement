using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class ReportsController : Controller
    {
        public async Task<IActionResult> MedicineReport()
        {
            return View();
        }

        public async Task<IActionResult> MedicineInventoryReport()
        {
            return View();
        }

        public async Task<IActionResult> AdmissionsReport()
        {
            return View();
        }

        public async Task<IActionResult> AppointmentsReport()
        {
            return View();
        }
    }
}
