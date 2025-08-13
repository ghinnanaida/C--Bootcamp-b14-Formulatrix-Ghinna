using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class BookCreateDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public required string Title { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
        public required string Author { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Page count must be a non-negative number.")]
        public int PageCount { get; set; }
    }
}