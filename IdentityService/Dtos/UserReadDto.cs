namespace Dtos
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public bool Status { get; set; }
    }
}