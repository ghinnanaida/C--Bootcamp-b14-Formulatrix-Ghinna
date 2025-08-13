using BookJournal.Models;

namespace BookJournal.Services.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
    }
}