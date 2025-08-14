using AutoMapper;
using BookJournal.Data;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookJournal.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IGenreRepository genreRepository, ApplicationDbContext context, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreDTO>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            var genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(genres);
            return genreDtos;
        }

        public async Task CreateBookAsync(BookCreateDTO createDto)
        {
            var book = _mapper.Map<Book>(createDto);
            book.Genres = await _genreRepository.GetGenresByIdsAsync(createDto.GenreIds);
            await _bookRepository.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<BookUpdateDTO?> GetBookForUpdateAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            var result = book == null ? null : _mapper.Map<BookUpdateDTO>(book);
            return result;
        }

        public async Task UpdateBookAsync(BookUpdateDTO updateDto)
        {
            var book = await _context.Books.Include(b => b.Genres).SingleOrDefaultAsync(b => b.Id == updateDto.Id);
            if (book == null) return;

            _mapper.Map(updateDto, book);
            var updatedGenres = await _genreRepository.GetGenresByIdsAsync(updateDto.GenreIds);
            book.Genres = updatedGenres;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book != null)
            {
                _bookRepository.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}