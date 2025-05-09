using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using BackendLogicApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendLogicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authService, JwtService jwtService)
        {
            _authService = authService;
           _jwtService = jwtService;
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
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal == null || principal.FindFirst("verify")?.Value != "true")
                return BadRequest("Nieprawidłowy lub przeterminowany token");

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Brak adresu e-mail");


            return Ok("Email został zweryfikowany!");
        }


    }
}
