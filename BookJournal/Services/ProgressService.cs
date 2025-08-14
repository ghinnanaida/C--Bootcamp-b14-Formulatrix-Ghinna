using AutoMapper;
using BookJournal.Data;
using BookJournal.DTOs;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;
using BookJournal.Services.Interfaces;
using BookJournal.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace BookJournal.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressTrackerRepository _progressTrackerRepository;
        private readonly IBookNoteRepository _bookNoteRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProgressService(IProgressTrackerRepository progressTrackerRepository, IBookNoteRepository bookNoteRepository, ApplicationDbContext context, IMapper mapper)
        {
            _progressTrackerRepository = progressTrackerRepository;
            _bookNoteRepository = bookNoteRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddToJournalAsync(ProgressTrackerCreateDTO dto, int userId)
        {
            var existingTracker = await _progressTrackerRepository.FindAsync(
                t => t.UserId == userId && t.BookId == dto.BookId);

            if (existingTracker.Any())
            {
                var result = false;
                return result; ;
            }

            var progressTracker = _mapper.Map<ProgressTracker>(dto);
            progressTracker.UserId = userId;

            if (dto.Status == BookStatus.Completed)
            {
                progressTracker.CurrentValue = dto.TotalValue;
                if (dto.EndDate == null) progressTracker.EndDate = DateTime.UtcNow;
            }
            await _progressTrackerRepository.AddAsync(progressTracker);
            await _context.SaveChangesAsync(); 
            var finalResult = true;
            return finalResult;
        }

        public async Task<bool> UpdateProgressAsync(ProgressTrackerUpdateDTO dto, int userId)
        {
            var tracker = await _context.ProgressTrackers.FirstOrDefaultAsync(t => t.Id == dto.Id && t.UserId == userId);
            if (tracker == null)
            {
                var result = false;
                return result; ;
            }
            var originalStatus = tracker.Status;
            _mapper.Map(dto, tracker);

            if (tracker.CurrentValue >= tracker.TotalValue)
            {
                tracker.CurrentValue = tracker.TotalValue;
                tracker.Status = BookStatus.Completed;
                if (tracker.EndDate == null)
                {
                    tracker.EndDate = DateTime.UtcNow;
                }
            }
            if (originalStatus == BookStatus.NotStarted && tracker.CurrentValue > 0)
            {
                tracker.Status = BookStatus.InProgress;
                if (tracker.StartDate == null) tracker.StartDate = DateTime.UtcNow;
            }
            if (originalStatus == BookStatus.Completed && tracker.CurrentValue < tracker.TotalValue)
            {
                tracker.Status = BookStatus.InProgress;
                if (tracker.StartDate == null) tracker.StartDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            var finalResult = true;
            return finalResult;
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
        
        public async Task<BookNoteDTO> AddNoteAsync(BookNoteCreateDTO dto, int userId)
        {
            var tracker = await _progressTrackerRepository.GetByIdAsync(dto.ProgressTrackerId);
            if (tracker == null || tracker.UserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot add a note to this book.");
            }

            var note = _mapper.Map<BookNotes>(dto);
            await _bookNoteRepository.AddAsync(note);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookNoteDTO>(note);
        }

        public async Task<bool> DeleteNoteAsync(int noteId, int userId)
        {
            var note = await _context.BookNotes
                .Include(n => n.ProgressTracker)
                .FirstOrDefaultAsync(n => n.Id == noteId);

            if (note == null || note.ProgressTracker.UserId != userId)
            {
                return false; 
            }

            _bookNoteRepository.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}