using AutoMapper;
using BookJournal.Repositories.Interfaces;
using BookJournal.DTOs;
using BookJournal.Enumerations;
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

            var validTrackers = userTrackers.Where(t => t.Book != null);

            var dashboardDto = new DashboardDTO
            {
                BooksInProgress = validTrackers.Count(t => t.Status == BookStatus.InProgress),
                BooksCompleted = validTrackers.Count(t => t.Status == BookStatus.Completed),
                BooksOnHold = validTrackers.Count(t => t.Status == BookStatus.OnHold),
                BooksDropped = validTrackers.Count(t => t.Status == BookStatus.Dropped),
                AllBooks = _mapper.Map<IEnumerable<ProgressTrackerDTO>>(validTrackers)
            };

            return dashboardDto;
        }
    }
}