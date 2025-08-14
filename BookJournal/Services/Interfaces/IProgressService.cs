using BookJournal.DTOs;

namespace BookJournal.Services.Interfaces
{
    public interface IProgressService
    {
        Task<bool> AddToJournalAsync(ProgressTrackerCreateDTO dto, int userId);
        Task<bool> UpdateProgressAsync(ProgressTrackerUpdateDTO dto, int userId);
        Task<ProgressDetailDTO?> GetProgressDetailAsync(int progressId, int userId);
        Task<ProgressTrackerUpdateDTO?> GetProgressForUpdateAsync(int progressId, int userId);
        Task DeleteProgressAsync(int progressId, int userId);
        Task<BookNoteDTO> AddNoteAsync(BookNoteCreateDTO dto, int userId);
        Task<bool> DeleteNoteAsync(int noteId, int userId);
    }
}