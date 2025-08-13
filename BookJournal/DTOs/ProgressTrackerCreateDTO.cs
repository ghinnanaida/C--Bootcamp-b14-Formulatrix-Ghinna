using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerCreateDTO
    {
        [Required]
        public int BookId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Current page must be a non-negative integer.")]
        public int CurrentPage { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "Total pages must be a positive integer.")]
        public int TotalPages { get; set; } = 1;
    }
}