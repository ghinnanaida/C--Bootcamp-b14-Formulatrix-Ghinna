using BookJournal.Data;
using BookJournal.Models;
using BookJournal.Repositories.Interfaces;

namespace BookJournal.Repositories
{
    public class BookNoteRepository : IBookNoteRepository
    {
        private readonly ApplicationDbContext _context;

        public BookNoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookNotes entity)
        {
            await _context.BookNotes.AddAsync(entity);
        }

        public async Task<BookNotes?> GetByIdAsync(int id)
        {
            return await _context.BookNotes.FindAsync(id);
        }

        public void Remove(BookNotes entity)
        {
            _context.BookNotes.Remove(entity);
        }
    }
}
