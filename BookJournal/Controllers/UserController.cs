using BookJournal.Models;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookJournal.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            SignInManager<User> signInManager,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetByEmailAsync(User.Identity?.Name ?? string.Empty);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userService.GetByEmailAsync(User.Identity?.Name ?? string.Empty);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userService.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} changed their password successfully", user.UserName);
                TempData["SuccessMessage"] = "Password changed successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string email, string userName)
        {
            var user = await _userService.GetByEmailAsync(User.Identity?.Name ?? string.Empty);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = email;
            user.UserName = userName;
            var result = await _userService.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} updated their profile successfully", user.UserName);
                TempData["SuccessMessage"] = "Profile updated successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction(nameof(Profile));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageRoles(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userService.GetRolesAsync(user);
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = new[] { "User", "Admin" }; 

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(int userId, List<string> roles)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userService.GetRolesAsync(user);
            
            foreach (var role in userRoles)
            {
                if (!roles.Contains(role))
                {
                    await _userService.RemoveFromRoleAsync(user, role);
                }
            }

            foreach (var role in roles)
            {
                if (!await _userService.IsInRoleAsync(user, role))
                {
                    await _userService.AddToRoleAsync(user, role);
                }
            }

            _logger.LogInformation("User {Username} roles updated successfully", user.UserName);
            TempData["SuccessMessage"] = "User roles updated successfully.";
            return RedirectToAction(nameof(ManageRoles), new { userId });
        }
    }
}