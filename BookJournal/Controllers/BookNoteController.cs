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
        private readonly IUserService _userService;

        public BookNoteController(IProgressService progressService, IUserService userService)
        {
            _progressService = progressService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] BookNoteCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userService.GetCurrentUserId(User);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var newNote = await _progressService.AddNoteAsync(dto, userId.Value);
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
            var userId = _userService.GetCurrentUserId(User);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var success = await _progressService.DeleteNoteAsync(id, userId.Value);
            if (!success)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}