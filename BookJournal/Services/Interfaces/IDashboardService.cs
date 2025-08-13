using BookJournal.DTOs;

namespace BookJournal.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardDataAsync(int userId);
    }
}