using BusBooking.Core.Interfaces;

using Microsoft.EntityFrameworkCore;
using BusBooking.Core.Entities;
using BusBooking.Infrastructure.Data;

using System.Linq;

namespace BusBooking.Infrastructure.Repositories
{
    public class BookingRepository:IBookingRepository
    {

        private readonly BusBookDbContext _context;

        public BookingRepository(BusBookDbContext context)
        {
            _context = context;
        }
        public  async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            
             
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings.
                Where(b => b.UserId == userId).
                Include(b => b.Bus).ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings.Include(b => b.Bus).Include(b => b.User).
                FirstOrDefaultAsync(b => b.Id == id);
        }
        

        public async Task<bool> IsSeatBooked(int busId, int seatNumber)
        {
            return await _context.Bookings.AnyAsync(b => b.BusId == busId && b.SeatNumber == seatNumber&&b.Status!="Cancelled");
        }

        public  Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);

            return Task.CompletedTask;
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Bus).Include(b=>b.User).ToListAsync();
        }
    }
}
