using BusBooking.Core.DTOs;
using BusBooking.Core.Entities;
using BusBooking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BookingController :ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService bookingService)
        {
            _service = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> Book(int busId, int userId, int seatNumber,PaymentMethod method)
        {
            var res = await _service.BookSeat(busId, userId, seatNumber,method);

            if (!res)
            {
                return BadRequest(new { message = "Seat is already booked" });

            }

            return Ok(new {message= "Seat booked successfully" });

        }

        [HttpGet("bus/{busId}/seats")]
        public async Task<IActionResult> GetSeatLayout(int busId)
        {
            var seats = await _service.GetSeatLayout(busId);

            return Ok(seats);
        }

        [HttpGet("user/{id}")]

        public async Task<IActionResult> GetBookingsByUser(int id)
        {
            var bookings = await _service.GetUserBookings(id);

            //if (bookings == null || bookings.Count == 0)
            //{
            //    return NotFound("No bookings found for the user.");
            //}

            return Ok(bookings?? new List<BookingDto>());
        }

        [HttpPut("cancel/{id}")]

        public async Task<IActionResult> CancelYourBooking(int id)
        {

            var res = await _service.CancelBooking(id);
            if (!res)
            {
                return BadRequest(new { message = "Invalid Booking or already Cancelled" });
            }

            return Ok(new { message = "Booking Cancelled Successfully" });
        }

    }
}
