using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public DateTime? PublishedDate { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}