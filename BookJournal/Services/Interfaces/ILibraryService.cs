using BookJournal.DTOs;

namespace BookJournal.Services.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
    }
}