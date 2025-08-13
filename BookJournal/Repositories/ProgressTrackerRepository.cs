using BookJournal.Repositories.Interfaces;
using BookJournal.Models;
using BookJournal.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookJournal.Repositories
{
    public class ProgressTrackerRepository : IProgressTrackerRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgressTrackerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProgressTracker entity)
        {
            await _context.ProgressTrackers.AddAsync(entity);
        }

        public async Task<IEnumerable<ProgressTracker>> FindAsync(Expression<Func<ProgressTracker, bool>> predicate)
        {
            return await _context.ProgressTrackers.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<ProgressTracker>> GetAllAsync()
        {
            return await _context.ProgressTrackers.ToListAsync();
        }

        public async Task<ProgressTracker?> GetByIdAsync(int id)
        {
            return await _context.ProgressTrackers.FindAsync(id);
        }

        public async Task<IEnumerable<ProgressTracker>> GetTrackersForUserAsync(int userId)
        {
            return await _context.ProgressTrackers
                .Include(pt => pt.Book)
                    .ThenInclude(b => b.Genres)
                .Where(pt => pt.UserId == userId)
                .OrderByDescending(pt => pt.LastStatusChangeDate)
                .ToListAsync();
        }

        public async Task<ProgressTracker?> GetTrackerWithDetailsAsync(int progressTrackerId, int userId)
        {
            return await _context.ProgressTrackers
                .Include(pt => pt.Book)
                    .ThenInclude(b => b.Genres)
                .Where(pt => pt.UserId == userId)
                .FirstOrDefaultAsync(pt => pt.Id == progressTrackerId);
        }

        public void Remove(ProgressTracker entity)
        {
            _context.ProgressTrackers.Remove(entity);
        }
    }
}
