using BookJournal.Models;

namespace BookJournal.Repositories.Interfaces
{
    public interface IBookNoteRepository
    {
        Task<BookNotes?> GetByIdAsync(int id);
        Task AddAsync(BookNotes entity);
        void Remove(BookNotes entity);
    }
}
