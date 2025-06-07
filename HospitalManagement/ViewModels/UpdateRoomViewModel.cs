using HospitalManagement.Models;
using System.ComponentModel;

namespace HospitalManagement.ViewModels
{
    public class UpdateRoomViewModel
    {
        public int RoomId { get; set; }

        [DisplayName("Room number")]
        public string RoomNumber { get; set; }

        [DisplayName("Department")]
        public Department Department { get; set; }

        [DisplayName("Number of beds")]
        public int NoOfBeds { get; set; }

        [DisplayName("Room status")]
        public RoomStatus Status { get; set; }
    }
}
