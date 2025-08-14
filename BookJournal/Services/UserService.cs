using BookJournal.Models;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BookJournal.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with email {Email}", email);
                throw;
            }
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created new user {Username} with ID {UserId}", user.UserName, user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to create user {Username}. Errors: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Username}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Updated user {Username} (ID: {UserId})", user.UserName, user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to update user {Username}. Errors: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Username} (ID: {UserId})", user.UserName, user.Id);
                throw;
            }
        }

        public async Task<IdentityResult> DeleteAsync(int userId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent user with ID {UserId}", userId);
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Deleted user {Username} (ID: {UserId})", user.UserName, userId);
                }
                else
                {
                    _logger.LogWarning("Failed to delete user {Username}. Errors: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            try
            {
                var result = await _userManager.CheckPasswordAsync(user, password);
                if (!result)
                {
                    _logger.LogWarning("Failed password check for user {Username}", user.UserName);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking password for user {Username}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password changed successfully for user {Username}", user.UserName);
                }
                else
                {
                    _logger.LogWarning("Failed to change password for user {Username}. Errors: {Errors}", 
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {Username}", user.UserName);
                throw;
            }
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            try
            {
                return await _userManager.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user {Username}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            try
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Added user {Username} to role {Role}", user.UserName, role);
                }
                else
                {
                    _logger.LogWarning("Failed to add user {Username} to role {Role}. Errors: {Errors}", 
                        user.UserName, role, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {Username} to role {Role}", user.UserName, role);
                throw;
            }
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            try
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Removed user {Username} from role {Role}", user.UserName, role);
                }
                else
                {
                    _logger.LogWarning("Failed to remove user {Username} from role {Role}. Errors: {Errors}", 
                        user.UserName, role, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {Username} from role {Role}", user.UserName, role);
                throw;
            }
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            try
            {
                return await _userManager.IsInRoleAsync(user, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {Username} is in role {Role}", user.UserName, role);
                throw;
            }
        }

        public int? GetCurrentUserId(ClaimsPrincipal user)
        {
            try
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    _logger.LogWarning("User ID claim not found in current principal");
                    return null;
                }

                if (int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }

                _logger.LogError("Failed to parse user ID {UserIdString} to integer", userIdString);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user ID from claims");
                return null;
            }
        }
    }
}