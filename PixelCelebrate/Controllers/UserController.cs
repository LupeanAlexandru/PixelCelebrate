using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelCelebrate.Data;
using PixelCelebrate.Models;
using PixelCelebrate.Services;
using System.Text.Json;


namespace PixelCelebrate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoginService _loginService;
        private readonly EmailService _emailService;
        private readonly BirthdayService _birthdayService;

        public UserController(ApplicationDbContext context, ILoginService loginService, EmailService emailService, BirthdayService birthdayService)
        {
            _context = context;
            _loginService = loginService;
            _emailService = emailService;
            _birthdayService = birthdayService;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            if (page < 1) page = 1;  
            if (pageSize < 1) pageSize = 15;  

            var totalUsers = await _context.Users.CountAsync();
            var users = await _context.Users
                                       .Skip((page - 1) * pageSize)  
                                       .Take(pageSize)  
                                       .ToListAsync();

            var result = new
            {
                users,
                totalUsers,
                totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                currentPage = page
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {

            user.Password = HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new {id = user.Id}, user);
        }
        private string HashPassword(string password)
        {  
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var authenticatedUser = await _loginService.AuthenticateUserAsync(user.Email, user.Password);
            if (authenticatedUser == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _loginService.GenerateJwtToken(authenticatedUser);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
            });
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPatch("{id}/isAdmin")]
        public async Task<IActionResult> UpdateIsAdmin(int id, [FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("isAdmin", out JsonElement isAdminElement))
            {
                return BadRequest("Invalid payload: 'isAdmin' property is required.");
            }

            bool isAdmin = isAdminElement.GetBoolean();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.IsAdmin = isAdmin;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("isAdmin status updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("send-notifications")]
        public async Task<IActionResult> SendBirthdayNotifications()
        {
            try
            {
                var daysBeforeBirthday = await _birthdayService.GetDaysBeforeBirthday();

                if(daysBeforeBirthday == null || daysBeforeBirthday < 1)
                {
                    return BadRequest("Number of days before birthday must be a positive integer.");
                }

                var users = await _context.Users.ToListAsync();

                var comingBirthday = await _emailService.SendBulkEmailAsync(users, daysBeforeBirthday.Value);

                if(comingBirthday)
                {
                    return Ok("Emails sent successfully.");
                } else 
                {
                    return Ok("No upcoming birthday.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
