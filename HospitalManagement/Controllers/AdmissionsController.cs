using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HospitalManagement.ViewModels;

namespace HospitalManagement.Controllers
{
    public class AdmissionsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Admissions()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AdmissionDetails()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AdmitPatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdmitPatient(AdmitPatientViewModel viewModel)
        {
            if (ModelState.IsValid) 
            { 
            
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAdmission()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdmission(UpdateAdmissionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

            }

            return View(viewModel);
        }
    }
}
