using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class BookDTO
    {
        
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new List<string>();
        public string? Publisher { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}