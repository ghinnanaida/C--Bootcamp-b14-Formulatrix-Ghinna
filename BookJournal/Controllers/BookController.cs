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
            ViewBag.AllGenres = await _bookService.GetAllGenresAsync();
            return View(new BookCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllGenres = await _bookService.GetAllGenresAsync();
                return View(createDto);
            }
            await _bookService.CreateBookAsync(createDto);
            return RedirectToAction("Index", "Library");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bookDto = await _bookService.GetBookForUpdateAsync(id);
            if (bookDto == null) return NotFound();
            
            ViewBag.AllGenres = await _bookService.GetAllGenresAsync();
            return View(bookDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllGenres = await _bookService.GetAllGenresAsync();
                return View(updateDto);
            }
            await _bookService.UpdateBookAsync(updateDto);
            return RedirectToAction("Index", "Library");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction("Index", "Library");
        }
    }
}