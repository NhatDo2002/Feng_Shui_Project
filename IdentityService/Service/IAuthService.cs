using Dtos;

namespace Service
{
    public interface IAuthService
    {
        Task LoginAsync(LoginRequestDto request);
        Task RegisterAsync(RegisterRequestDto request);
        Task RefreshTokenAsync(string refreshToken);
    }
}