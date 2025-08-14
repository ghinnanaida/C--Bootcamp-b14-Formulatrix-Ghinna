using BookJournal.DTOs;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookJournal.Controllers
{
    [Authorize]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return View(genres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Genre name is required and must not exceed 100 characters.";
                return RedirectToAction("Index");
            }

            var success = await _genreService.AddGenreAsync(dto);

            if (!success)
            {
                TempData["Error"] = "This genre already exists.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return RedirectToAction("Index");
        }
    }
}