namespace BusBooking.Core.Entities
{
    public  class User
    {
        public int Id { get; set; }
        public required string  UserName { get; set; }

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "User";

        public ICollection<Booking>? Bookings { get; set; }
    }
}
