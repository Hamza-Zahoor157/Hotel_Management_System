using Identity_Example.Models;
using Identity_Example.Models.Intefaces;
using Identity_Example.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Identity_Example.Controllers
{
    public class CustomerController : Controller
    {
        private IRoomRepository _roomRepository;
        private UserManager<MyAppUser> _userManager;


        public CustomerController(IRoomRepository roomRepository, UserManager<MyAppUser> userManager)
        {
            _roomRepository = roomRepository;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public ViewResult CustomerPage()
        {
            return View();
        }
        [AllowAnonymous]
        public ViewResult CustomerLogin()
        {
            return View();
        }
        [AllowAnonymous]
        public ViewResult CustomerSignup()
        {
            return View();
        }
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult CustomerDashboard()
        {
            return View();
        }
        [AllowAnonymous] 
        public ViewResult BookRoom()
        {
            return View();
        }
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult CancelReservation()
        {
            return View("CancelReservation");
        }
        //[HttpPost]
        //public ViewResult BookRoom(Booking room)
        //{
        //    string message;

        //    bool result = _roomRepository.BookRoom(room);

        //    if (result)
        //    {
        //        message = "Room Booked Successfully!";
        //    }
        //    else
        //    {
        //        message = "Booking Failed! Either customer ID is invalid or room is already booked for selected dates.";
        //    }
        //    return View("BookRoomResult", message);
        //}
        [HttpPost]
        [Authorize(Policy = "CustomerAccess")]
        public async Task<ViewResult> BookRoom(Booking booking)
        {
            string message;

            string cid = _userManager.GetUserId(User);
            booking.CustomerID = cid;
            bool result = await _roomRepository.BookRoom(booking);
            if (result)
            {
                message = "Room Booked Successfully!";
            }
            else
            {
                message = "Booking Failed! Either customer ID is invalid or room is already booked for selected dates.";
            }
            return View("BookRoomResult", message);
        }
        [HttpPost]
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult CancelReservation(string BookingID)
        {

            string message;

            string cid = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(BookingID) || !int.TryParse(BookingID, out int id))
            {
                message = "Invalid Booking ID...";
                return View("CancelReservationResult", message);
            }
            bool result = _roomRepository.CancelReservation(id,cid);
            if (result)
            {
                message = "Booking Cancelled Successfully!";
            }
            else
            {
                message = "Booking could not be cancelled..";
            }
            return View("CancelReservationResult", message);
        }
        [Authorize(Policy = "CustomerAccess")]  
        public ViewResult SearchRooms()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult SearchRooms(string RoomType, string minprice, string maxprice)
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            List<Room> searchedRooms = new List<Room>();
            int max, min;

            min = int.Parse(minprice);
            max = int.Parse(maxprice);
            foreach(var room in rooms)
            {
                if (room.RoomType.Equals(RoomType) && (room.Price >= min) && room.Price <=max && room.IsAvailable == true)
                {
                    searchedRooms.Add(room);
                }
            }

            return View("AvailableRooms", searchedRooms);
        }
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult AvailableRooms()
        {
            return View();
        }
        [Authorize(Policy = "CustomerAccess")]
        public ViewResult CancelReservationResult()
        {
            return View();
        }

        [Authorize(Policy = "CustomerAccess")]
        public ViewResult MyReservations()
        {
            string cid = _userManager.GetUserId(User);
            List<Booking> list = _roomRepository.GetMyReservations(cid);
            return View("MyReservations",list);
        }
    }
}
