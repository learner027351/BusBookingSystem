using BusBooking.Core.Entities;
using BusBooking.Infrastructure.Data;
using BusBooking.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly BusBookDbContext _context;
        private readonly AuthService _auth;
        private readonly JwtService _jwt;

        public AuthController(BusBookDbContext context,AuthService auth,JwtService jwt)
        {
            _context = context;
            _auth = auth;
            _jwt = jwt;
        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(string username,string password,string role)
        {
            if (_context.Users.Any(u => u.UserName == username))
                return BadRequest("User with same username already exists");

            if (password.Length<7)
            {
                Console.WriteLine("Password Length should be greater than 6");
            }

            var user = new User
            {
                UserName = username,
                PasswordHash = _auth.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return Ok("Registered Successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null || !_auth.VerifyPassword(password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwt.GenerateToken(user);

            return Ok(new { token, role = user.Role, userId = user.Id });
        }




    }
}
