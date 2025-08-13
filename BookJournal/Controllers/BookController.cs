using BookJournal.DTOs;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookJournal.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = await _bookService.GetGenresSelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = await _bookService.GetGenresSelectListAsync();
                return View(createDto);
            }

            await _bookService.CreateBookAsync(createDto);
            return RedirectToAction("Index", "Library");
        }
    }
}
