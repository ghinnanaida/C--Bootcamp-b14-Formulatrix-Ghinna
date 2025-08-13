using AutoMapper;
using BookJournal.Data;
using BookJournal.Repositories.Interfaces;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task CreateBookAsync(BookCreateDTO createDto)
        {
            var book = _mapper.Map<Book>(createDto);

            if (createDto.GenreIds.Any())
            {
                book.Genres = await _genreRepository.GetGenresByIdsAsync(createDto.GenreIds);
            }

            await _bookRepository.AddAsync(book);
            await _context.SaveChangesAsync(); // Save changes here
        }

        public async Task<SelectList> GetGenresSelectListAsync()
        {
            return new SelectList(await _genreRepository.GetAllAsync(), "Id", "Name");
        }
    }
}