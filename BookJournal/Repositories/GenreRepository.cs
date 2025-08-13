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
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<ICollection<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
        }

        public void Remove(Genre entity)
        {
            _context.Genres.Remove(entity);
        }
    }
}