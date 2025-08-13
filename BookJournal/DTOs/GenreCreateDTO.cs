using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class GenreCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

    }
}