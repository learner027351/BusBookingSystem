using BusBooking.Core.Interfaces;
using BusBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using BusBooking.Core.Entities;
using BusBooking.Core.DTOs;

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

        public async Task<bool> BookSeat(int busId, int userId, int seatNumber,PaymentMethod method)
        {
            //var exists = await _context.Bookings.AnyAsync(b => b.BusId == busId && b.SeatNumber == seatNumber);

            using var transaction=await _context.Database.BeginTransactionAsync();

            try
            {




                var isBooked = await _bookingrepo.IsSeatBooked(busId, seatNumber);



                if (isBooked) return false;


                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null) return false;

                int totalSeats = int.TryParse(bus.TotalSeats, out var seats) ? seats : 0;

                if (seatNumber > totalSeats) return false;

                var booking = new Booking
                {
                    BusId = busId,
                    UserId = userId,
                    SeatNumber = seatNumber,
                    Status = "Confirmed",
                    BookingTime = DateTime.UtcNow
                };

                //_context.Bookings.Add(booking);

                await _bookingrepo.AddAsync(booking);

                await _context.SaveChangesAsync();


                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = bus.Price, // make sure Price exists
                    PaymentMethod = method,
                    Status = "Success",
                    PaymentDate = DateTime.UtcNow
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }

            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return false;
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
        public async Task<bool> CancelBooking(int bookingId)
        {
            var booking=await _bookingrepo.GetByIdAsync(bookingId);

            if (booking == null) return false;

            if (booking.Status == "Cancelled") return false;

            booking.Status = "Cancelled";

            await _bookingrepo.UpdateAsync(booking);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
