using BookJournal.Repositories.Interfaces;
using BookJournal.Models;
using BookJournal.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookJournal.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Book entity)
        {
            await _context.Books.AddAsync(entity);
        }

        public async Task<IEnumerable<Book>> FindAsync(Expression<Func<Book, bool>> predicate)
        {
            return await _context.Books.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.Genres).OrderBy(b => b.Title).ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Genres).FirstOrDefaultAsync(b => b.Id == id);
        }

        public void Remove(Book entity)
        {
            _context.Books.Remove(entity);
        }
    }
}
