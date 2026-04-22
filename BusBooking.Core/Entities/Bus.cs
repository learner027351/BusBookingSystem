using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BusBooking.Core.Entities
{
    public  class Bus
    {
        public int Id { get; set; }

        [Required]
        public string BusNumber { get; set; } = null!;
        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;

        public string TotalSeats { get; set; } = null!;


        public DateTime TravelDate { get; set; }

        public TimeSpan TravelTime { get; set; }

        public decimal Price { get; set; }

        public ICollection<Booking>? Bookings { get; set; }

    }
}
