using Dtos;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContext;
        public AuthController(IAuthService authService, IHttpContextAccessor httpContext)
        {
            _authService = authService;
            _httpContext = httpContext;   
        }

        [Route("/api/auth/register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto requestDto)
        {
            await _authService.RegisterAsync(requestDto);

            return Ok();
        }

        [Route("/api/auth/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto requestDto)
        {
            await _authService.LoginAsync(requestDto);

            return Ok();
        }

        [Route("/api/auth/refresh")]
        [HttpGet]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = _httpContext.HttpContext!.Request.Cookies["REFRESH_TOKEN"];
            await _authService.RefreshTokenAsync(refreshToken!);
            return Ok();
        }

    }
}