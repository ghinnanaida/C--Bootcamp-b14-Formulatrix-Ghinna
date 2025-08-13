using BookJournal.Repositories.Interfaces;
using BookJournal.Models;
using BookJournal.Services.Interfaces;

namespace BookJournal.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;

        public LibraryService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }
    }
}