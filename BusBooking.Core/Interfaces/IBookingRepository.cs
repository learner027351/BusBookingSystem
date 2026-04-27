using BusBooking.Core.Entities;



namespace BusBooking.Core.Interfaces
{
    public interface IBookingRepository
    {

        Task AddAsync(Booking booking);
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<Booking?> GetByIdAsync(int id);
        Task<bool> IsSeatBooked(int busId, int seatNumber);

        Task UpdateAsync(Booking booking);

        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
    }
}
