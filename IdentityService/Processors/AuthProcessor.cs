using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Options;

namespace Processors
{
    public class AuthProcessor : IAuthProcessor
    {
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthProcessor(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        public (string jwtToken, DateTime expiresAtUtc) GenerateJWTToken(User user)
        {
            // var jwtSetting = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Value.Key);
            
            var signingKey = new SymmetricSecurityKey(key);

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString() ),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Name)
            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.DurationInMinutes);

            var getToken = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(getToken);

            // var tokenHandler = new JwtSecurityTokenHandler();
            // var token = tokenHandler.CreateToken(tokenDescriptor); 

            return (jwtToken, expires);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void WriteAuthTokenAtHttpOnlyCookie(string cookieName, string token, DateTime? expiration)
        {
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Expires = expiration,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}