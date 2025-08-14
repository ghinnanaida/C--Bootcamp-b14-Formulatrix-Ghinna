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
            var result = await _context.ProgressTrackers.Where(predicate).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<ProgressTracker>> GetAllAsync()
        {
            var trackers = await _context.ProgressTrackers.ToListAsync();
            return trackers;
        }

        public async Task<ProgressTracker?> GetByIdAsync(int id)
        {
            var tracker = await _context.ProgressTrackers.FindAsync(id);
            return tracker;
        }

        public async Task<IEnumerable<ProgressTracker>> GetTrackersForUserAsync(int userId)
        {
            var trackers = await _context.ProgressTrackers
                .Include(pt => pt.Book)
                    .ThenInclude(b => b.Genres)
                .Where(pt => pt.UserId == userId)
                .OrderByDescending(pt => pt.LastStatusChangeDate)
                .ToListAsync();
            return trackers;
        }

        public async Task<ProgressTracker?> GetTrackerWithDetailsAsync(int progressTrackerId, int userId)
        {
            var tracker = await _context.ProgressTrackers
                .Include(pt => pt.Book)
                    .ThenInclude(b => b.Genres)
                .Where(pt => pt.UserId == userId)
                .FirstOrDefaultAsync(pt => pt.Id == progressTrackerId);
            return tracker;
        }

        public void Remove(ProgressTracker entity)
        {
            _context.ProgressTrackers.Remove(entity);
        }
    }
}
