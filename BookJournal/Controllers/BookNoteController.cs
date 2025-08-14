using BookJournal.DTOs;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookJournal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookNoteController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public BookNoteController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] BookNoteCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdString);
            try
            {
                var newNote = await _progressService.AddNoteAsync(dto, userId);
                return Ok(newNote);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);
            var success = await _progressService.DeleteNoteAsync(id, userId);

            if (!success)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}