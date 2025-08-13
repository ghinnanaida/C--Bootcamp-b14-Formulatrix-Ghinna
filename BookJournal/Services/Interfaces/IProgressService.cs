using BookJournal.DTOs;

namespace BookJournal.Services.Interfaces
{
    public interface IProgressService
    {
        Task<bool> AddToJournalAsync(ProgressTrackerCreateDTO dto, int userId);
        Task<bool> UpdateProgressAsync(ProgressTrackerUpdateDTO dto, int userId);
    }
}