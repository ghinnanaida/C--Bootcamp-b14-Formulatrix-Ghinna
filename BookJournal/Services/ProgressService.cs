using AutoMapper;
using BookJournal.Data;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var tracker = await _context.ProgressTrackers.FirstOrDefaultAsync(t => t.Id == dto.Id && t.UserId == userId);
            if (tracker == null) return false;

            _mapper.Map(dto, tracker);
            if (dto.Status == Enumerations.BookStatus.Completed && tracker.EndDate == null)
            {
                tracker.EndDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProgressDetailDTO?> GetProgressDetailAsync(int progressId, int userId)
        {
            var tracker = await _context.ProgressTrackers
                .Include(pt => pt.Book).ThenInclude(b => b.Genres)
                .FirstOrDefaultAsync(pt => pt.Id == progressId && pt.UserId == userId);

            if (tracker == null) return null;

            return new ProgressDetailDTO
            {
                Tracker = _mapper.Map<ProgressTrackerDTO>(tracker),
                Book = _mapper.Map<BookDTO>(tracker.Book)
            };
        }

        public async Task<ProgressTrackerUpdateDTO?> GetProgressForUpdateAsync(int progressId, int userId)
        {
            var tracker = await _context.ProgressTrackers.FirstOrDefaultAsync(t => t.Id == progressId && t.UserId == userId);
            return tracker == null ? null : _mapper.Map<ProgressTrackerUpdateDTO>(tracker);
        }

        public async Task DeleteProgressAsync(int progressId, int userId)
        {
            var tracker = await _context.ProgressTrackers.FirstOrDefaultAsync(t => t.Id == progressId && t.UserId == userId);
            if (tracker != null)
            {
                _context.ProgressTrackers.Remove(tracker);
                await _context.SaveChangesAsync();
            }
        }
    }
}