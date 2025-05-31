using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HospitalManagement.ViewModels;
using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class AdmissionsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;

        public AdmissionsController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Admissions()
        {
            var admissions = await _context.Admissions
                .Include(a => a.Patient)
                .ToListAsync(); 

            return View(admissions);
        }

        [HttpGet]
        public async Task<IActionResult> AdmissionDetails()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AdmitPatient()
        {
            var viewModel = new AdmitPatientViewModel
            {
                
            };
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
