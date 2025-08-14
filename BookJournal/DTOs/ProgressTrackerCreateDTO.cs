using BookJournal.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerCreateDTO
    {
        [Required]
        public int BookId { get; set; }

        [Display(Name = "Reading Status")]
        public BookStatus Status { get; set; }

        [Display(Name = "Book Format")]
        public BookType BookType { get; set; }

        [Display(Name = "Progress Unit")]
        public ProgressUnit ProgressUnit { get; set; }

        [Display(Name = "Total (Pages/Percent/Episodes)")]
        public double TotalValue { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Your Rating (0-10)")]
        [Range(0, 10)]
        public double Rating { get; set; } 
        
        [Display(Name = "Current Progress")]
        public double CurrentValue { get; set; }
    }
}