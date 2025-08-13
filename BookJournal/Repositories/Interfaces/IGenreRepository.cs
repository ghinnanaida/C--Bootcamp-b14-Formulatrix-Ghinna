using BookJournal.Models;

namespace BookJournal.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<Genre?> GetByIdAsync(int id);
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<ICollection<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids);
        Task AddAsync(Genre entity);
        void Remove(Genre entity);
    }
}