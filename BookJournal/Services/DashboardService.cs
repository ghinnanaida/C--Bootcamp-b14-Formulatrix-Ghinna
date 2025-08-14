using AutoMapper;
using BookJournal.DTOs;
using BookJournal.Enumerations;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;

namespace BookJournal.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProgressTrackerRepository _progressTrackerRepository;
        private readonly IMapper _mapper;

        public DashboardService(IProgressTrackerRepository progressTrackerRepository, IMapper mapper)
        {
            _progressTrackerRepository = progressTrackerRepository;
            _mapper = mapper;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync(int userId)
        {
            var userTrackers = await _progressTrackerRepository.GetTrackersForUserAsync(userId);
            var validTrackers = userTrackers.Where(t => t.Book != null && !string.IsNullOrEmpty(t.Book.Title));
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var completedLastWeek = validTrackers
                .Where(t => t.Status == BookStatus.Completed && t.EndDate >= sevenDaysAgo);

            var pagesReadLastWeek = validTrackers
                .Where(t => t.ProgressUnit == ProgressUnit.Pages && t.LastStatusChangeDate >= sevenDaysAgo)
                .Sum(t => t.CurrentValue); 

            var genreChartData = validTrackers
                .SelectMany(t => t.Book.Genres)
                .GroupBy(g => g.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var monthlyCompletionData = validTrackers
                .Where(t => t.Status == BookStatus.Completed && t.EndDate >= sixMonthsAgo)
                .GroupBy(t => new { t.EndDate!.Value.Year, t.EndDate!.Value.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToDictionary(x => x.Month.ToString("MMM yyyy"), x => x.Count);


            var dashboardDto = new DashboardDTO
            {
                BooksInProgress = validTrackers.Count(t => t.Status == BookStatus.InProgress),
                BooksCompleted = validTrackers.Count(t => t.Status == BookStatus.Completed),
                BooksOnHold = validTrackers.Count(t => t.Status == BookStatus.OnHold),
                BooksDropped = validTrackers.Count(t => t.Status == BookStatus.Dropped),
                AllBooks = _mapper.Map<IEnumerable<ProgressTrackerDTO>>(validTrackers),
                BooksCompletedLastWeek = completedLastWeek.Count(),
                PagesReadLastWeek = pagesReadLastWeek,
                GenreChartData = genreChartData,
                TbrBooks = _mapper.Map<IEnumerable<ProgressTrackerDTO>>(validTrackers.Where(t => t.Status == BookStatus.NotStarted)),
                InProgressBooks = _mapper.Map<IEnumerable<ProgressTrackerDTO>>(validTrackers.Where(t => t.Status == BookStatus.InProgress || t.Status == BookStatus.OnHold)),
                CompletedBooks = _mapper.Map<IEnumerable<ProgressTrackerDTO>>(validTrackers.Where(t => t.Status == BookStatus.Completed || t.Status == BookStatus.Dropped)),
                MonthlyCompletionChartData = monthlyCompletionData

            };

            return dashboardDto;
        }
    }
}
