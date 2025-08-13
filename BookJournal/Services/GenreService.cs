using AutoMapper;
using BookJournal.Data;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;

namespace BookJournal.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genreRepository, ApplicationDbContext context, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreDTO>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GenreDTO>>(genres);
        }

        public async Task AddGenreAsync(GenreCreateDTO dto)
        {
            var genre = _mapper.Map<Genre>(dto);
            await _genreRepository.AddAsync(genre); // Use repository to add
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenreAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre != null)
            {
                _genreRepository.Remove(genre); // Use repository to remove
                await _context.SaveChangesAsync();
            }
        }
    }
}