using BusBooking.Core.DTOs;
using BusBooking.Core.Entities;
using BusBooking.Core.Interfaces;
using BusBooking.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusBooking.Infrastructure.Services
{
    public class BookingService:IBookingService
    {
        private readonly IBookingRepository _bookingrepo;
        private readonly BusBookDbContext _context;

        public  BookingService(IBookingRepository bookingrepo,BusBookDbContext context)
        {
            _bookingrepo = bookingrepo;
            _context = context;
        }

        //public async Task<bool> BookSeat(int busId, int userId, int seatNumber,PaymentMethod method)
        //{


        //    using var transaction=await _context.Database.BeginTransactionAsync();

        //    try
        //    {






        //        var bus = await _context.Buses.FindAsync(busId);
        //        if (bus == null) return false;

        //        int totalSeats = int.TryParse(bus.TotalSeats, out var seats) ? seats : 0;

        //        if (seatNumber > totalSeats) return false;

        //        var booking = new Booking
        //        {
        //            BusId = busId,
        //            UserId = userId,
        //            SeatNumber = seatNumber,
        //            Status = "Confirmed",
        //            BookingTime = DateTime.UtcNow
        //        };

        //        //_context.Bookings.Add(booking);

        //        await _bookingrepo.AddAsync(booking);

        //        await _context.SaveChangesAsync();


        //        var payment = new Payment
        //        {
        //            BookingId = booking.Id,
        //            Amount = bus.Price, // make sure Price exists
        //            PaymentMethod = method,
        //            Status = "Success",
        //            PaymentDate = DateTime.UtcNow
        //        };

        //        await _context.Payments.AddAsync(payment);
        //        await _context.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //        return true;
        //    }

        //    catch (DbUpdateException ex)
        //    {


        //        await transaction.RollbackAsync();

        //        if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        //        {
        //            return false; 
        //        }
        //        throw;
        //    }
        //}
        public async Task<BookingResult> BookSeat(int busId, int userId, int seatNumber, PaymentMethod method)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null)
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = "No bus found with this ID"
                    };
                }

                int totalSeats = int.TryParse(bus.TotalSeats, out var seats) ? seats : 0;

                if (seatNumber <= 0 || seatNumber > totalSeats)
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = "Invalid seat number"
                    };
                }

                var booking = new Booking
                {
                    BusId = busId,
                    UserId = userId,
                    SeatNumber = seatNumber,
                    Status = "Confirmed",
                    BookingTime = DateTime.UtcNow
                };

                await _bookingrepo.AddAsync(booking);
                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = bus.Price,
                    PaymentMethod = method,
                    Status = "Success",
                    PaymentDate = DateTime.UtcNow
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new BookingResult
                {
                    Success = true,
                    Message = "Seat booked successfully"
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                if (ex.InnerException?.Message.Contains("IX_Bookings_BusId_SeatNumber") == true)
                {
                    return new BookingResult
                    {
                        Success = false,
                        Message = "Seat already booked"
                    };
                }

                return new BookingResult
                {
                    Success = false,
                    Message = "Database error occurred"
                };
            }
        }

        public async Task<List<SeatDto>>GetSeatLayout(int busId)
        {
            var bus = await _context.Buses.FindAsync(busId);
            if(bus==null)return new List<SeatDto>();


            int totalSeats = Convert.ToInt32(bus.TotalSeats);

            var bookedSeats = await _context.Bookings
                .Where(b => b.BusId == busId && b.Status != "Cancelled")
                .Select(b => b.SeatNumber)
                .ToListAsync();

            var seatLayout = new List<SeatDto>();

            for (int i = 1; i <= totalSeats; i++)
            {
                seatLayout.Add(new SeatDto
                {
                    SeatNumber = i,
                    IsBooked = bookedSeats.Contains(i)
                });
            }
            return seatLayout;
        }

        public async Task<List<BookingDto>> GetUserBookings(int userId)
        {
            var bookings = await _context.Bookings.Where(b => b.UserId == userId)
                .Include(b => b.Bus)
                .Include(b => b.User)
                .Include(b => b.Payment)
                .ToListAsync();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                SeatNumber = b.SeatNumber,
                Status = b.Status,

                BusNumber = b.Bus.BusNumber,
                Source = b.Bus.Source,
                Destination = b.Bus.Destination,
                UserName = b.User.UserName,
                BookingTime = b.BookingTime,
                PaymentMethod = b.Payment != null
                        ? b.Payment.PaymentMethod.ToString()
                        : "N/A"

            }).ToList();


        }
        public async Task<bool> CancelBooking(int bookingId,int userId)
        {
            var booking=await _bookingrepo.GetByIdAsync(bookingId);

            if (booking == null||booking.UserId!=userId) return false;

            if (booking.Status == "Cancelled") return false;

            booking.Status = "Cancelled";

            await _bookingrepo.UpdateAsync(booking);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
