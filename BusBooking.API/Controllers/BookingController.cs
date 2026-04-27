 using BusBooking.Core.DTOs;
using BusBooking.Core.Entities;
using BusBooking.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Book(int busId, int seatNumber, PaymentMethod method)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //var res = await _service.BookSeat(busId, userId, seatNumber, method);

            //if (!res)
            //    return BadRequest(new { message = "Seat is already booked" });

            //return Ok(new { message = "Seat booked successfully" });
            var result = await _service.BookSeat(busId, userId, seatNumber, method);

            if (!result.Success)
                return BadRequest(new {  result.Message });

            return Ok(new {  result.Message });
        }
            

        [HttpGet("bus/{busId}/seats")]
        public async Task<IActionResult> GetSeatLayout(int busId)
        {
            var seats = await _service.GetSeatLayout(busId);
            

            return Ok(seats);
        }

        
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var bookings = await _service.GetUserBookings(userId);

            return Ok(bookings ?? new List<BookingDto>());
        }

        [HttpPut("cancel/{id}")]

        public async Task<IActionResult> CancelYourBooking(int id)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var res = await _service.CancelBooking(id,userId);
            if (!res)
            {
                return BadRequest(new { message = "Invalid Booking or already Cancelled" });
            }

            return Ok(new { message = "Booking Cancelled Successfully" });
        }

    }
}
