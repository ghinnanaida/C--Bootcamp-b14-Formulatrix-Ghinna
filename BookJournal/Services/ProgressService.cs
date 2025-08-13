using AutoMapper;
using BookJournal.Data;
using BookJournal.Repositories.Interfaces;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Services.Interfaces;

namespace BookJournal.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressTrackerRepository _progressTrackerRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProgressService(IProgressTrackerRepository progressTrackerRepository, ApplicationDbContext context, IMapper mapper)
        {
            _progressTrackerRepository = progressTrackerRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddToJournalAsync(ProgressTrackerCreateDTO dto, int userId)
        {
            var existingTracker = await _progressTrackerRepository.FindAsync(
                t => t.UserId == userId && t.BookId == dto.BookId);

            if (existingTracker.Any())
            {
                return false;
            }

            var progressTracker = _mapper.Map<ProgressTracker>(dto);
            progressTracker.UserId = userId;

            await _progressTrackerRepository.AddAsync(progressTracker);
            await _context.SaveChangesAsync(); // Save changes here
            return true;
        }

        public async Task<bool> UpdateProgressAsync(ProgressTrackerUpdateDTO dto, int userId)
        {
            var tracker = await _progressTrackerRepository.GetByIdAsync(dto.Id);

            if (tracker == null || tracker.UserId != userId)
            {
                return false;
            }

            _mapper.Map(dto, tracker);

            if (dto.Status == Enumerations.BookStatus.Completed && tracker.EndDate == null)
            {
                tracker.EndDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(); // Save changes here
            return true;
        }
    }
}
