using BookJournal.Models;
using System.Linq.Expressions;

namespace BookJournal.Repositories.Interfaces
{
    public interface IProgressTrackerRepository
    {
        Task<ProgressTracker?> GetByIdAsync(int id);
        Task<IEnumerable<ProgressTracker>> GetAllAsync();
        Task<IEnumerable<ProgressTracker>> FindAsync(Expression<Func<ProgressTracker, bool>> predicate);
        Task AddAsync(ProgressTracker entity);
        void Remove(ProgressTracker entity);
        Task<IEnumerable<ProgressTracker>> GetTrackersForUserAsync(int userId);
        Task<ProgressTracker?> GetTrackerWithDetailsAsync(int progressTrackerId, int userId);
    }
}