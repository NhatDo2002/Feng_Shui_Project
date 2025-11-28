using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string? Name { get; set; }
        public string? RefreshToken {get; set;}
        public DateTime? RefreshTokenExpiresAtUtc {get; set;}
        public static User Create(string email, string name)
        {
            return new User
            {
                UserName = email,
                Email = email,
                Name = name,
            };
        }
    }
}