using BookJournal.DTOs;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookJournal.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);
            var detailDto = await _progressService.GetProgressDetailAsync(id, userId);
            if (detailDto == null) return NotFound();
            return View(detailDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToJournal(ProgressTrackerCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["Error"] = string.Join(", ", errors);
                return RedirectToAction("Index", "Library");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdString);
            var success = await _progressService.AddToJournalAsync(dto, userId);

            if (!success)
            {
                TempData["Error"] = "This book is already in your journal.";
                return RedirectToAction("Index", "Library");
            }

            TempData["Success"] = "Book successfully added to your journal.";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProgressTrackerUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["Error"] = string.Join(", ", errors);
                return RedirectToAction("Details", new { id = dto.Id });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdString);
            var success = await _progressService.UpdateProgressAsync(dto, userId);

            if (!success)
            {
                TempData["Error"] = "Could not find the progress tracker to update.";
                return RedirectToAction("Details", new { id = dto.Id });
            }

            TempData["Success"] = "Progress successfully updated.";
            return RedirectToAction("Details", new { id = dto.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);
            await _progressService.DeleteProgressAsync(id, userId);
            TempData["Success"] = "Progress tracker successfully deleted.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
