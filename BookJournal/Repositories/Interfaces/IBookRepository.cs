using BookJournal.Models;
using System.Linq.Expressions;

namespace BookJournal.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> FindAsync(Expression<Func<Book, bool>> predicate);
        Task AddAsync(Book entity);
        void Remove(Book entity);
    }
}
