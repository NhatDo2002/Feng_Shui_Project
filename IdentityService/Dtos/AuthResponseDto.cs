namespace Dtos
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserReadDto User { get; set; }
        public string Message { get; set; }
    }
}