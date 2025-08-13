using BookJournal.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookJournal.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<GenreDTO>> GetAllGenresAsync();
        Task CreateBookAsync(BookCreateDTO createDto);
        Task<BookUpdateDTO?> GetBookForUpdateAsync(int id);
        Task UpdateBookAsync(BookUpdateDTO updateDto);
        Task DeleteBookAsync(int id);
    }
}