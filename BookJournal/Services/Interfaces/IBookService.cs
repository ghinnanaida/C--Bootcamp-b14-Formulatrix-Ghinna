using BookJournal.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookJournal.Services.Interfaces
{
    public interface IBookService
    {
        Task<SelectList> GetGenresSelectListAsync();
        Task CreateBookAsync(BookCreateDTO createDto);
    }
}