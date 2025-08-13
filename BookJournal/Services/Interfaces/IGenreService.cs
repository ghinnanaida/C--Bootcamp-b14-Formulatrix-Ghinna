using BookJournal.DTOs;

namespace BookJournal.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDTO>> GetAllGenresAsync();
        Task AddGenreAsync(GenreCreateDTO dto);
        Task DeleteGenreAsync(int id);
    }
}