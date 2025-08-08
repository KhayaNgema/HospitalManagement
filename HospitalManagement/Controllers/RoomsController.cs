using HospitalManagement.Data;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HospitalManagementDbContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IEncryptionService _encryptionService;
        private readonly EmailService _emailService;
        private readonly QrCodeService _qrCodeService;
        private readonly DeviceInfoService _deviceInfoService;
        private readonly FileUploadService _fileUploadService;

        public RoomsController(HospitalManagementDbContext context,
            UserManager<UserBaseModel> userManager,
            IEncryptionService encryptionService,
            EmailService emailService,
            FileUploadService fileUploadService,
            DeviceInfoService deviceInfoService,
            QrCodeService qrCodeService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _fileUploadService = fileUploadService;
            _qrCodeService = qrCodeService;
            _deviceInfoService = deviceInfoService;

        }

        [HttpGet]
        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.CreatedBy)
                .Include(r => r.ModifiedBy)
                .ToListAsync();

            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Room(string roomId)
        {
            var decryptedRoomId = _encryptionService.DecryptToInt(roomId);

            var room = await _context.Rooms
                .Where(r => r.RoomId == decryptedRoomId)
                .FirstOrDefaultAsync();

            return View(room);
        }


        [HttpGet]
        public async Task<IActionResult> NewRoom()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRoom(Room model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var existingRoom = await _context.Rooms
                    .Where(er => er.RoomNumber == model.RoomNumber &&
                    er.Department == model.Department)
                    .FirstOrDefaultAsync();

                if (existingRoom != null)
                {
                    TempData["Message"] = $"You cannot add/create a room with the same name in the {model.Department} department.";

                    return View(model);
                }

                var newRoom = new Room
                {
                    RoomNumber = model.RoomNumber,
                    Department = model.Department,
                    NoOfBeds = model.NoOfBeds,
                    Status = RoomStatus.Available,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    LastUpdatedAt = DateTime.Now,
                    UpdatedById = user.Id,
                    UsedBeds = 0
                };

                _context.Add(newRoom);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully added room {model.RoomNumber} for the {model.Department} department.";

                return RedirectToAction(nameof(Rooms));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to create a new room: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRoom(string roomId)
        {
            var decryptedRoomId = _encryptionService.DecryptToInt(roomId);

            var room = await _context.Rooms
                .Where(r => r.RoomId == decryptedRoomId)
                .FirstOrDefaultAsync();

            var viewModel = new UpdateRoomViewModel
            {
                RoomId = decryptedRoomId,
                Department = room.Department,
                NoOfBeds = room.NoOfBeds,
                Status = room.Status,
                RoomNumber = room.RoomNumber,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoom(UpdateRoomViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var room = await _context.Rooms
                    .Where(r => r.RoomId == viewModel.RoomId)
                    .FirstOrDefaultAsync();

                room.Department = viewModel.Department;
                room.Status = viewModel.Status;
                room.NoOfBeds = viewModel.NoOfBeds;

                _context.Update(room);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"You have successfully updated room {room.RoomNumber} details.";

                return RedirectToAction(nameof(Rooms));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to update room: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                });
            }

            return View();
        }
    }
}
