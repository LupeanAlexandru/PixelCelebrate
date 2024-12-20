using Microsoft.AspNetCore.Mvc;
using PixelCelebrate.Data;
using Microsoft.EntityFrameworkCore;
using PixelCelebrate.Models;

namespace PixelCelebrate.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class BirthdayController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BirthdayController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetBirthday()
        {
            var daysBeforeBirthday = await _context.Birthdays.FindAsync(1);
            if(daysBeforeBirthday == null)
            {
                return NotFound("Birthday configuration not found.");
            }

            return Ok(daysBeforeBirthday);
        }

        [HttpPatch]

        public async Task<ActionResult> UpdateDaysBeforeBirthday([FromHeader] int daysBeforeBirthday)
        {
            var daysToBirthday = await _context.Birthdays.FindAsync(1);
            if (daysToBirthday == null)
            {
                return NotFound("Birthday configuration not found.");
            }

            daysToBirthday.DaysBeforeBirthday = daysBeforeBirthday;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("DaysBeforeBirthday updated successfully.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while updating the database: " + ex.Message);
            }

        }
    }
}
