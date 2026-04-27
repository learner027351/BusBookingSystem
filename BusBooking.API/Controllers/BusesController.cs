using BusBooking.Core.DTOs;
using BusBooking.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusesController : ControllerBase
    {
        private readonly IBusService _service;

        public BusesController(IBusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var buses = await _service.GetAllBuses();
            return Ok(buses);
        }

        [HttpGet("search")]

        public async Task<IActionResult> Search(string source, string destination,DateTime date)
        {
            var buses = await _service.SearchBuses(source, destination,date);
            return Ok(buses);
        }

        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBus([FromBody] CreateBusDto dto) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var res=await _service.AddBus(dto);

            Console.WriteLine(User.Identity.IsAuthenticated);
            Console.WriteLine(User.FindFirst(ClaimTypes.Role)?.Value);
            return Ok(new { message = "Bus added successfully" });
        }

        //[AllowAnonymous]

        [Authorize(Roles= "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id) { 


            var res=await _service.DeleteBus(id);

            if (!res)
                return NotFound("Bus not found");

            return Ok("Bus Deleted Successfully");
        
        }

    }
}
