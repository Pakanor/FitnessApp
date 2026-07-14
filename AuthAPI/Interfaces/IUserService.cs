using AuthAPI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthAPI.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task UpdateProfileAsync(ClaimsPrincipal principal, string newUsername, string newEmail, DateTime? birthDate = null, decimal? currentWeight = null, decimal? height = null, string? gender = null, string? jobType = null, string? goal = null);
        Task ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
        Task DeleteAccountAsync(ClaimsPrincipal principal);
        Task SendPasswordResetLinkAsync(string email);
    }
}
