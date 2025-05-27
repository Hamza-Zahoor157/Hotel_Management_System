using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity_Example.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        
        [Required]
        [StringLength(450)]
        public string CustomerID { get; set; }
        public int RoomID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        [ForeignKey("CustomerID")]
        public MyAppUser Customer { get; set; }

        [ForeignKey("RoomID")]
        public Room Room { get; set; }
    }
}
