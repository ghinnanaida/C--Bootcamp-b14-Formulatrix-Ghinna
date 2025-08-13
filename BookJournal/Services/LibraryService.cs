using AutoMapper;
using BookJournal.DTOs;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;

namespace BookJournal.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public LibraryService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDTO>>(books);
        }
    }
}
