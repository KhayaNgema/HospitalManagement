using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class AppointmentsController : Controller
    {

        [HttpGet]
        public async Task<IActionResult> AllAppointments()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyTeamAppointments()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentDetails(string appointmentId)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MakeAppointment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakeAppointment(MakeAppointmentViewModel viewModel)
        {
            if (ModelState.IsValid) 
            { 

            }

            return View(viewModel);
        }
    }
}
