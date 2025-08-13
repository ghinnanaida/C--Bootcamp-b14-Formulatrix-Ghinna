using BookJournal.Models;

namespace BookJournal.Repositories;
public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllGenresAsync();
    Task<Genre?> GetGenreByIdAsync(int id);
    Task AddGenreAsync(Genre genre);
    Task UpdateGenreAsync(Genre genre);
    Task DeleteGenreAsync(int id);
}