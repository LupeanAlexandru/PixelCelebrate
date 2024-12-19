using PixelCelebrate.Data;

namespace PixelCelebrate.Services
{
    public class BirthdayService
    {
        private readonly ApplicationDbContext _context;

        public  BirthdayService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetDaysBeforeBirthday()
        {
            var birthdayConfig = await _context.Birthdays.FindAsync(1);
            return birthdayConfig?.DaysBeforeBirthday;
        }
    }
}
