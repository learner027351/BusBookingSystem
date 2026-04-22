using BusBooking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
