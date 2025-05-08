using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var tokenString = await _authService.LoginAsync(dto);
                if (string.IsNullOrEmpty(tokenString))
                {
                    return Unauthorized("Nieprawidłowy login lub hasło.");
                }
                Response.Headers.Add("Location", "/home");
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("protected-resource")]
        public IActionResult GetProtectedResource()
        {
            return Ok(new { message = "Dostęp do chronionego zasobu." });
        }


    }
}
