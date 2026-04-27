

namespace BusBooking.Core.Entities
{
    public class Booking
    {

        public int Id { get; set; }
        public int BusId { get; set; }
        public int UserId { get; set; }

        public int SeatNumber { get; set; }

        public Bus? Bus { get; set; }

        public User? User { get; set; }

        public Payment? Payment { get; set; }

        public string Status { get; set; } = "Confirmed";


        public DateTime BookingTime { get; set; }
    }
}
