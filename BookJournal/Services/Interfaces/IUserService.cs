using BookJournal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookJournal.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IdentityResult> UpdateAsync(User user);
        Task<IdentityResult> DeleteAsync(int userId);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(User user, string role);
        Task<bool> IsInRoleAsync(User user, string role);
        int? GetCurrentUserId(ClaimsPrincipal user);
    }
}