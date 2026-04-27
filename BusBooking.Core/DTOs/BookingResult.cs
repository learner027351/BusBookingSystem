using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.DTOs
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
