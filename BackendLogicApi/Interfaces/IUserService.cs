using BackendLogicApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendLogicApi.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task UpdateProfileAsync(ClaimsPrincipal principal, string newUsername, string newEmail);
        Task ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
        Task DeleteAccountAsync(ClaimsPrincipal principal);
        Task SendPasswordResetLinkAsync(string email);
    }
}
