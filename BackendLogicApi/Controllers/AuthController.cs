using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLogicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok(new { message = "Rejestracja zakończona pomyślnie." });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            await _authService.LoginAsync(dto);
            return Ok(new { message = "zalogowano" });
        }


    }
}
