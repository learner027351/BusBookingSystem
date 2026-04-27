

namespace BusBooking.Core.DTOs
{
    public class CreateBusDto
    {
        public string BusNumber { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string Destination { get; set; } = null!;

        public int TotalSeats { get; set; }

        public DateTime TravelDate { get; set; }
        public TimeSpan TravelTime { get; set; }

        public decimal Price { get; set; }
    }
}
