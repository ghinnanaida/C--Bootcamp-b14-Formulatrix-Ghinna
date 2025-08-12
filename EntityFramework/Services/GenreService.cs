using EntityFramework.Models;
using EntityFramework.Data;
using Microsoft.EntityFrameworkCore;    

namespace EntityFramework.Services
{
    public class GenreService
    {
        private readonly CinemaDbContext _context;

        public GenreService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllGenreAsync()
        {
            return await _context.Genres        
                .Include(m => m.Movies)
                .ToListAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _context.Genres
                .Include(m => m.Movies)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddGenreAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenreAsync(int id)
        {
            var genre = await GetGenreByIdAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }
    }
}