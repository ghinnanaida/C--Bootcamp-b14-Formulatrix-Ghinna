using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookJournal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserService _userService;

        public DashboardController(IDashboardService dashboardService, IUserService userService)
        {
            _dashboardService = dashboardService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetCurrentUserId(User);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var dashboardDto = await _dashboardService.GetDashboardDataAsync(userId.Value);
            return View(dashboardDto);
        }
    }
}
