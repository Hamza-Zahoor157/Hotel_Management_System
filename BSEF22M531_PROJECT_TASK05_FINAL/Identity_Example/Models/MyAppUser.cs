using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Identity_Example.Models
{
    public class MyAppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string City { get; set; }
        public string RoleType { get; set; }

        public List<Booking> Bookings { get; set; }
    }
}