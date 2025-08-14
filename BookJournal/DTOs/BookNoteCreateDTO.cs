using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class BookNoteCreateDTO
    {
        [Required]
        public int ProgressTrackerId { get; set; }

        [Required]
        [MinLength(1)]
        [Display(Name = "Your Note or Quote")]
        public string Note { get; set; } = string.Empty;
    }
}