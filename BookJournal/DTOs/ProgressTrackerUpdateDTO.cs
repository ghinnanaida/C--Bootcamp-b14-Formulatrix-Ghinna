using BookJournal.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public double CurrentValue { get; set; }

        [Required]
        public BookStatus Status { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }
    }
}