using Identity_Example.Data;
using Identity_Example.Hubs;
using Identity_Example.Models.Intefaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Identity_Example.Models.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RoomRepository(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public bool AddRoom(Room room)
        {
            try
            {
                _context.Rooms.Add(room);
                int changes = _context.SaveChanges();
                if (changes > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<Room> GetAllRooms()
        {
            return _context.Rooms.ToList();
        }


        public async Task<bool> BookRoom(Booking booking)
        {

            var customer = await _context.Users.FindAsync(booking.CustomerID);
            if (customer == null)
                return false;

            if (booking.CheckInDate > booking.CheckOutDate)
            {
                return false;
            }
            try
            {
                //var userExists = await _context.Users.AnyAsync(u => u.Id == booking.CustomerID && u.RoleType == "Customer");
                //if (!userExists)
                //    return false;

                var isRoomAvailable = !await _context.Bookings.AnyAsync(b => b.RoomID == booking.RoomID &&
                         !(b.CheckOutDate < booking.CheckInDate ||
                           b.CheckInDate > booking.CheckOutDate));

                if (!isRoomAvailable)
                    return false;
                booking.Customer = customer;
                await _context.Bookings.AddAsync(booking);

                var room = await _context.Rooms.FindAsync(booking.RoomID);
                if (room != null)
                {
                    room.IsAvailable = false;
                    _context.Rooms.Update(room);
                }

                var changes = await _context.SaveChangesAsync();

                // Send notification after successful booking
                if (changes > 0)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New booking: Room {booking.RoomID} by {customer.FullName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error booking room: " + ex.Message);
                return false;
            }
        }
        public List<Booking> GetAllBookedRooms()
        {
            return _context.Bookings
                           .Include(b => b.Customer)   
                           .Include(b => b.Room)      
                           .ToList();
        }

        public List<Booking> GetMyReservations(string customerid)
        {
            return _context.Bookings.Where(c=> c.CustomerID == customerid).Include(c=>c.Customer).Include(c=>c.Room).ToList();
        }
        public bool CancelReservation(int BookingID, string customerid)
        {
            try
            {
                var booking = _context.Bookings.Find(BookingID); 
                if (booking == null)
                    return false;

                if(customerid != booking.CustomerID)
                {
                    return false;
                }

                var room = _context.Rooms.Find(booking.RoomID);
                if (room != null)
                {
                    room.IsAvailable = true;
                    _context.Rooms.Update(room);
                }

                _context.Bookings.Remove(booking);

                var changes = _context.SaveChanges();
                if (changes > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error canceling reservation: " + ex.Message);
                return false;
            }

        }

        public bool RemoveRoom(int roomid)
        {
            try
            {
                var room = _context.Rooms.Find(roomid);
                if (room == null)
                {
                    return false;
                }
                var booking = _context.Bookings.Where(r => r.RoomID == room.RoomID);
                if (booking.Any())
                {
                    return false;
                }
                _context.Rooms.Remove(room);
                int changes = _context.SaveChanges();
                if (changes > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateRoom(Room room)
        {
            try
            {
                //var room1 = _context.Rooms.Find(room.RoomID);
                //if(room1 == null)
                //{
                //    return false;
                //}

                //room1.RoomType = room.RoomType;
                //room1.Price = room.Price;
                //room1.IsAvailable = room.IsAvailable;

                var booking = _context.Bookings.Where(r => r.RoomID == room.RoomID);
                if (booking.Any())
                {
                    return false;
                }
                _context.Rooms.Update(room);
                int changes = _context.SaveChanges();
                if (changes > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        //public bool AddRoom(Room room)
        //{
        //    string? connstring = @"Data Source=(localdb)\MSSQLLocalDB;
        //                        Initial Catalog=HMS_DB;
        //                        Integrated Security=True;";
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connstring))
        //        {
        //            connection.Open();
        //            string query = "INSERT INTO rooms(RoomType, Price, Availability) VALUES(@RoomType, @Price, @Availability)";
        //            using (SqlCommand cmd = new SqlCommand(query, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
        //                cmd.Parameters.AddWithValue("@Price", room.Price);
        //                cmd.Parameters.AddWithValue("@Availability", room.IsAvailable ? 1 : 0);

        //                int res = cmd.ExecuteNonQuery();
        //                if (res > 0)
        //                {
        //                    return true;
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return false; //If true not returned there is an issue...
        //}

        //public List<Room> GetAllRooms()
        //{
        //    List<Room> rooms = new List<Room>();
        //    string? connstring = @"Data Source=(localdb)\MSSQLLocalDB;
        //                        Initial Catalog=HMS_DB;
        //                        Integrated Security=True;";

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connstring))
        //        {
        //            connection.Open();
        //            string? query = $"SELECT * FROM rooms";
        //            using (SqlCommand cmd = new SqlCommand(query, connection))
        //            {
        //                SqlDataReader reader = cmd.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    Room room = new Room();
        //                    room.RoomID = Convert.ToInt32(reader[0]);
        //                    room.RoomType = Convert.ToString(reader[1]);
        //                    room.Price = Convert.ToInt32(reader[2]);
        //                    room.IsAvailable = Convert.ToBoolean(reader[3]);

        //                    rooms.Add(room);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return rooms;
        //}

        //public bool BookRoom(Booking room)
        //{
        //    string connstring = @"Data Source=(localdb)\MSSQLLocalDB;
        //                  Initial Catalog=HMS_DB;
        //                  Integrated Security=True;";
        //    string customerId = room.CustomerID!;
        //    string? customerName = room.CustomerName;
        //    int roomId = room.RoomID;

        //    DateTime checkIn = room.CheckInDate;
        //    DateTime checkOut = room.CheckOutDate;

        //    if(checkIn > checkOut)
        //    {
        //        return false;
        //    }

        //    try
        //    {

        //        using (SqlConnection connection = new SqlConnection(connstring))
        //        {
        //            connection.Open();
        //            string checkUser = "SELECT Id FROM AspNetUsers WHERE Id = @CustomerID";

        //            using (SqlCommand checkUsercmd = new SqlCommand(checkUser, connection))
        //            {
        //                checkUsercmd.Parameters.AddWithValue("@CustomerID", customerId);
        //                using (SqlDataReader reader = checkUsercmd.ExecuteReader())
        //                {
        //                    if (!reader.HasRows)
        //                    {
        //                        return false; // No customer with such Id exists....
        //                    }
        //                }
        //            }
        //            string checkQuery = @"SELECT COUNT(*) FROM Bookings 
        //                                    WHERE RoomID = @RoomID AND NOT (
        //                                    CheckOutDate < @CheckInDate OR 
        //                                    CheckInDate > @CheckOutDate)";

        //            using (SqlCommand checkBookingsCmd = new SqlCommand(checkQuery, connection))
        //            {
        //                checkBookingsCmd.Parameters.AddWithValue("@RoomID", roomId);
        //                checkBookingsCmd.Parameters.AddWithValue("@CheckInDate", checkIn);
        //                checkBookingsCmd.Parameters.AddWithValue("@CheckOutDate", checkOut);

        //                int count = (int)checkBookingsCmd.ExecuteScalar();

        //                if (count > 0)
        //                {
        //                    return false; 
        //                }
        //            }

        //            string insertQuery = @"INSERT INTO Bookings (CustomerID, RoomID, CustomerName, CheckInDate, CheckOutDate) VALUES (@CustomerID, @RoomID, @CustomerName, @CheckInDate, @CheckOutDate)";

        //            using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
        //            {
        //                insertCmd.Parameters.AddWithValue("@CustomerID", customerId);
        //                insertCmd.Parameters.AddWithValue("@RoomID", roomId);
        //                insertCmd.Parameters.AddWithValue("@CustomerName", customerName);
        //                insertCmd.Parameters.AddWithValue("@CheckInDate", checkIn.Date);
        //                insertCmd.Parameters.AddWithValue("@CheckOutDate", checkOut.Date);

        //                int res = insertCmd.ExecuteNonQuery();
        //                if (res == 0)
        //                {
        //                    return false;
        //                }
        //            }

        //            string Updatequery = "UPDATE rooms SET Availability = 0 WHERE RoomID = @RoomID";
        //            using (SqlCommand cmd = new SqlCommand(Updatequery, connection))
        //            {

        //                cmd.Parameters.AddWithValue("@RoomID", roomId);
        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error booking room: " + ex.Message);
        //        return false;
        //    }
        //    return true;
        //}


        //public List<Booking> GetAllBookedRooms()
        //{
        //    List<Booking> bookings = new List<Booking> ();


        //    string? connstring = @"Data Source=(localdb)\MSSQLLocalDB;
        //                        Initial Catalog=HMS_DB;
        //                        Integrated Security=True;";
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connstring))
        //        {
        //            string query = @"SELECT * FROM Bookings"; 
        //            SqlCommand command = new SqlCommand(query, conn);
        //            conn.Open();
        //            SqlDataReader reader = command.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                Booking booking = new Booking();

        //                booking.BookingID = Convert.ToInt32(reader[0]);
        //                booking.CustomerID = reader[1].ToString();
        //                booking.RoomID = Convert.ToInt32(reader[2]);
        //                booking.CustomerName = reader[3].ToString();
        //                booking.CheckInDate = Convert.ToDateTime(reader[4]);
        //                booking.CheckOutDate = Convert.ToDateTime(reader[5]);

        //                bookings.Add(booking);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return bookings;
        //}



    }
}
