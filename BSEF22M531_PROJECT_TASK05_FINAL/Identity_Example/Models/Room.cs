using System.ComponentModel.DataAnnotations;

namespace Identity_Example.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoomType { get; set; }
    
        public int Price { get; set; }

        public bool IsAvailable { get; set; }

        public List<Booking> Bookings { get; set; }

    }
}
