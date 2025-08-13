using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class GenreCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}