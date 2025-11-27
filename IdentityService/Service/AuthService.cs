using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Data;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(AppDbContext context, IConfiguration configuration, IMapper mapper )
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if(user != null)
            {
                if (VerifyHashedPassword(request.Password, user.PasswordHash!))
                {
                    var token = GenerateJWTToken(request.Username);
                    return new AuthResponseDto()
                    {
                        Success = true,
                        AccessToken = token,
                        RefreshToken = token,
                        User = _mapper.Map<UserReadDto>(user),
                        Message = "Đăng nhập thành công"
                    };
                }
            }
            return new AuthResponseDto()
            {
                Success = false,
                Message = "Tài khoản hoặc mật khấu không đúng"
            };
        }

        private bool VerifyHashedPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string HashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJWTToken(string username)
        {
            var jwtSetting = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSetting["DurationInMinutes"])),
                Issuer = jwtSetting["Issuer"],
                Audience = jwtSetting["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public Task<AuthResponseDto> RefreshTokenAsync(LoginRequestDto request)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponseDto> RegisterAsync(UserCreateDto request)
        {
            var checkUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (checkUser)
            {
                return new AuthResponseDto()
                {
                    Success = false,
                    Message = "Tên tài khoản đã tồn tại. Vui lòng nhập tên khác"
                };
            }
            var newUser = _mapper.Map<>
            throw new NotImplementedException();
        }
    }
}