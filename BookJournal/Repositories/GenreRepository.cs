using BookJournal.Data;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookJournal.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Genre entity)
        {
            await _context.Genres.AddAsync(entity);
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return genres;
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            return genre;
        }

        public async Task<ICollection<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids)
        {
            var genres = await _context.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
            return genres;
        }

        public void Remove(Genre entity)
        {
            _context.Genres.Remove(entity);
        }
    }
}