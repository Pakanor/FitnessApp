using AuthAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthAPI.Models;
using AuthAPI.Services;
using AuthAPI.Infrastructure;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : UserHeaderControllerBase
    {
        private readonly IUserService _userService;
        

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            var user = await _userService.GetCurrentUserAsync(userId.Value);
            if (user == null) return NotFound();
            return Ok(new ProfileResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                BirthDate = user.BirthDate,
                CurrentWeight = user.CurrentWeight,
                Height = user.Height,
                Gender = user.Gender,
                JobType = user.JobType,
                Goal = user.Goal,
                Bmr = user.GetBmr(),
                Tdee = user.GetTdee()
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            await _userService.UpdateProfileAsync(userId.Value, dto.Username, dto.Email, dto.BirthDate, dto.CurrentWeight, dto.Height, dto.Gender, dto.JobType, dto.Goal);
            return NoContent();
        }


        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            await _userService.ChangePasswordAsync(userId.Value, dto.CurrentPassword, dto.NewPassword);
            return NoContent();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            await _userService.DeleteAccountAsync(userId.Value);
            return NoContent();
        }
        [HttpPost("send-reset-password-email")]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] EmailDto dto)
        {
            await _userService.SendPasswordResetLinkAsync(dto.Email);
            return Ok("Wysłano wiadomość z linkiem do resetu hasła.");
        }

    }

}
