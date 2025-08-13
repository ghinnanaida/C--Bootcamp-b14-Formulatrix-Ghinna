using BookJournal.Models;

namespace BookJournal.Repositories;
public interface IProgressTrackerRepository
{
    Task<IEnumerable<ProgressTracker>> GetAllProgressTrackersAsync();
    Task<ProgressTracker?> GetProgressTrackerByIdAsync(int id);
    Task AddProgressTrackerAsync(ProgressTracker progressTracker);
    Task UpdateProgressTrackerAsync(ProgressTracker progressTracker);
    Task DeleteProgressTrackerAsync(int id);
}