using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; } = null!;

        public string BusNumber { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string Destination { get; set; } = null!;

        public string UserName { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;

        public DateTime BookingTime { get; set; }
    }
}
