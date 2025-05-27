using AspNetCoreGeneratedDocument;

namespace Identity_Example.Models.Intefaces
{
    public interface IRoomRepository
    {
        public bool AddRoom(Room room);

        public bool RemoveRoom(int roomid);

        public bool UpdateRoom(Room room);
        public List<Room> GetAllRooms();

        //public bool BookRoom(Booking room);
        Task<bool> BookRoom(Booking booking);

        public bool CancelReservation(int bookingid, string customerid);

        public List<Booking> GetAllBookedRooms();

        public List<Booking> GetMyReservations(string customerid);
       
    }
}
