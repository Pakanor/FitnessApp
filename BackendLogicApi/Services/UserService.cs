using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using System.Security.Claims;
using BackendLogicApi.Models;


namespace BackendLogicApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserLogrepository _userRepo;
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;
        public UserService(UserLogrepository userRepo, JwtService jwtService, IEmailService emailService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            int userId = _jwtService.GetUserIdFromClaims(principal);
            return await _userRepo.GetByIdAsync(userId);
        }

        public async Task UpdateProfileAsync(ClaimsPrincipal principal, string newUsername, string newEmail)
        {
            var user = await GetCurrentUserAsync(principal);
            if (user == null) throw new Exception("Użytkownik nie istnieje");

            user.Username = newUsername;
            user.Email = newEmail;

            await _userRepo.UpdateUserAsync(user);
        }

        public async Task ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
        {
            var user = await GetCurrentUserAsync(principal);
            if (user == null) throw new Exception("Użytkownik nie istnieje");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new Exception("Nieprawidłowe aktualne hasło.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepo.UpdateUserAsync(user);
        }

        public async Task DeleteAccountAsync(ClaimsPrincipal principal)
        {
            var user = await GetCurrentUserAsync(principal);
            if (user == null) throw new Exception("Użytkownik nie istnieje");

            await _userRepo.DeleteUserAsync(user);
        }      

        public async Task SendPasswordResetLinkAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Użytkownik o podanym adresie e-mail nie istnieje.");

            var token = _jwtService.GeneratePasswordResetToken(user);
            var resetLink = $"http://localhost:5142/api/user/reset-password?token={token}";

            string subject = "Resetowanie hasła";
            string body = $"Kliknij <a href=\"{resetLink}\">tutaj</a>, aby zresetować swoje hasło. Link ważny przez 15 minut.";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }


    }

}
