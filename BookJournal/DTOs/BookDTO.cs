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

        public string? Genre { get; set; }

        public DateTime? PublicationDate { get; set; }

        public string? Summary { get; set; }
    }
}