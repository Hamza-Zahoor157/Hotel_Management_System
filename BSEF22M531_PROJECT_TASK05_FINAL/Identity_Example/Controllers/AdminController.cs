using Identity_Example.Models;
using Identity_Example.Models.Intefaces;
using Identity_Example.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;

namespace Identity_Example.Controllers
{
    public class AdminController : Controller
    {
        private IRoomRepository _roomRepository;
        public  AdminController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        [AllowAnonymous]
        public ViewResult AdminLogin()
        {
            return View();
        }
        [Authorize(Policy = "AdminAccess")]
        public ViewResult AdminDashboard()
        {
            return View();
        }
        [Authorize(Policy = "AdminOrManager")]
        public ViewResult AddRoomDetails()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "AdminOrManager")]
        public ViewResult AddRoomDetails(string RoomType, string Price, string Availability)
        {
            int price = int.Parse(Price);

            bool isAvail = false;
            if (!string.IsNullOrEmpty(Availability))
            {
                if (Availability.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                Availability.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                Availability == "1")
                {
                    isAvail = true;
                }
            }
            else
            {
                isAvail = false;
            }

            Room room = new Room();
            room.RoomType = RoomType;
            room.Price = price;
            room.IsAvailable = isAvail;


            
            bool res = _roomRepository.AddRoom(room);
            string message;
            if (res)
            {
                message = "Room Inserted Successfully...";
            }
            else
            {
                message = "Room could not be Inserted...";

            }
            return View("AddRoomResult", message);
        }
        [HttpPost]
        [Authorize(Policy = "AdminOrManager")]
        public ViewResult RemoveRoomDetails(string RoomID)
        {
            int roomid = int.Parse(RoomID);

            bool res = _roomRepository.RemoveRoom(roomid);
            string message;
            if (res)
            {
                message = "Room Removed Successfully...";
            }
            else
            {
                message = "Room could not be Removed...";

            }
            return View("RemoveRoomResult", message);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrManager")]
        public ViewResult UpdateRoomDetails(string RoomID, string RoomType, string Price, string Availability)
        {
            int price = int.Parse(Price);
            int roomid = int.Parse(RoomID);
            bool isAvail = false;
            if (!string.IsNullOrEmpty(Availability))
            {
                if (Availability.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                Availability.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                Availability == "1")
                {
                    isAvail = true;
                }
            }
            else
            {
                isAvail = false;
            }

            Room room = new Room();
            room.RoomID = roomid;
            room.RoomType = RoomType;
            room.Price = price;
            room.IsAvailable = isAvail;



            bool res = _roomRepository.UpdateRoom(room);
            string message;
            if (res)
            {
                message = "Room Updated Successfully...";
            }
            else
            {
                message = "Room could not be Updated...";

            }
            return View("UpdateRoomResult", message);
        }
        [Authorize(Policy = "AdminOrManager")]
        public ViewResult ViewReservationDetails()
        {
            List<Booking> list = _roomRepository.GetAllBookedRooms();


            return View("ViewReservationDetails",list);
        }

        [Authorize(Policy = "AdminOrManager")]
        public ViewResult RemoveRoomDetails()
        {
            return View();
        }
        [Authorize(Policy = "AdminAccess")]
        public ViewResult Notifications()
        {
            return View();
        }

        [Authorize(Policy = "AdminOrManager")]
        public ViewResult UpdateRoomDetails()
        {
            return View();
        }
    }
}
