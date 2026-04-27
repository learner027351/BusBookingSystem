using BusBooking.Core.DTOs;
using BusBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.Interfaces
{
    public interface IBookingService
    {
        //Task<bool> BookSeat(int busId,int userId,int seatNumber,PaymentMethod method);

        Task<BookingResult> BookSeat(int busId, int userId, int seatNumber, PaymentMethod method);
        Task<List<BookingDto>> GetUserBookings(int userId);

        Task<bool> CancelBooking(int bookingId,int userId);
        Task<List<SeatDto>> GetSeatLayout(int busId);
    }
}
