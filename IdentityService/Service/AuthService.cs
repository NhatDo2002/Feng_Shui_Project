using Dtos;
using Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;
using Processors;
using Repositories;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProcessor _authProcessor;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public AuthService(IAuthProcessor authProcessor, UserManager<User> userManager, IUserRepository userRepository )
        {
            _authProcessor = authProcessor;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new LoginFailException();
            }

            var (jwtToken, expirationDateInUTC) = _authProcessor.GenerateJWTToken(user);
            var refreshToken = _authProcessor.GenerateRefreshToken();

            var refreshTokenExpirationDateInUTC = DateTime.UtcNow.AddDays(7);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUTC;

            await _userManager.UpdateAsync(user);

            _authProcessor.WriteAuthTokenAtHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUTC);
            _authProcessor.WriteAuthTokenAtHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, user.RefreshTokenExpiresAtUtc);
        }

        public async Task RegisterAsync(RegisterRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new EmptyNameException();
            }
            if (string.IsNullOrEmpty(request.Email))
            {
                throw new EmptyEmailException();
            }
        
            var userExists = await _userManager.FindByEmailAsync(request.Email) != null;
            if (userExists)
            {
                throw new UserAlreadyExistException(request.Email);
            }
            var user = User.Create(request.Email, request.Name);
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new RegistrationFailException(result.Errors.Select(x => x.Description));
            }
        }

        public async Task RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new RefreshTokenException("Refresh token bị trống");
            }
            var user = await _userRepository.GetUserByRefreshToken(refreshToken);
            if(user == null)
            {
                throw new RefreshTokenException("Không thể lấy được refresh token của người dùng");
            }
            if(user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                throw new RefreshTokenException("Refresh token của người dùng đã hết hạn");
            }
        }
    }
}