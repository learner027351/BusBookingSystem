using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.Entities
{
    public  class User
    {
        public int Id { get; set; }
        public required string  UserName { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
