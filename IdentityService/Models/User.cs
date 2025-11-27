using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Phone { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public bool Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        
    }
}