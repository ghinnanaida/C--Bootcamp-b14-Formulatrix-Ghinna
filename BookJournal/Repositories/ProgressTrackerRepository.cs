using Microsoft.EntityFrameworkCore;
using BookJournal.Models;
using BookJournal.Data;

namespace BookJournal.Repositories;
public class ProgressTrackerRepository : IProgressTrackerRepository
{
    private readonly ApplicationDbContext _context;

    public ProgressTrackerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProgressTracker>> GetAllProgressTrackersAsync()
    {
        return await _context.ProgressTrackers.ToListAsync();
    }

    public async Task<ProgressTracker?> GetProgressTrackerByIdAsync(int id)
    {
        return await _context.ProgressTrackers.FindAsync(id);
    }

    public async Task AddProgressTrackerAsync(ProgressTracker progressTracker)
    {
        _context.ProgressTrackers.Add(progressTracker);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProgressTrackerAsync(ProgressTracker progressTracker)
    {
        _context.ProgressTrackers.Update(progressTracker);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProgressTrackerAsync(int id)
    {
        var progressTracker = await GetProgressTrackerByIdAsync(id);
        if (progressTracker != null)
        {
            _context.ProgressTrackers.Remove(progressTracker);
            await _context.SaveChangesAsync();
        }
    }
}