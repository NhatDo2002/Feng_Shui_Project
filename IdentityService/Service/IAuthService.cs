using Dtos;

namespace Service
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(LoginRequestDto request);
    }
}