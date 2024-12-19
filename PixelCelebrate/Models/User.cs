using System.ComponentModel.DataAnnotations;

namespace PixelCelebrate.Models
{
    public class User
    {
        public int Id {  get; set; }

        public required string Email { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public required string Password { get; set; }

        public Boolean IsAdmin { get; set; } = false; 

        public DateTime BirthDay { get; set; }

    }
}
