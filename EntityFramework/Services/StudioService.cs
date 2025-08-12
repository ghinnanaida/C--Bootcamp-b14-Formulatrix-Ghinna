using EntityFramework.Models;
using EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class StudioService
    {
        private readonly CinemaDbContext _context;

        public StudioService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Studio>> GetAllStudiosAsync()
        {
            return await _context.Studios
                .Include(s => s.ShowTime)
                .ToListAsync();
        }

        public async Task<Studio?> GetStudioByIdAsync(int id)
        {
            return await _context.Studios
                .Include(s => s.ShowTime)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddStudioAsync(Studio studio)
        {
            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudioAsync(Studio studio)
        {
            _context.Studios.Update(studio);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudioAsync(int id)
        {
            var studio = await GetStudioByIdAsync(id);
            if (studio != null)
            {
                _context.Studios.Remove(studio);
                await _context.SaveChangesAsync();
            }
        }
    }
}