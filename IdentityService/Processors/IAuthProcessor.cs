using Models;

namespace Processors
{
    public interface IAuthProcessor
    {
        (string jwtToken, DateTime expiresAtUtc) GenerateJWTToken(User user);
        string GenerateRefreshToken();
        void WriteAuthTokenAtHttpOnlyCookie(string cookieName, string token, DateTime? expiration);
    }
}