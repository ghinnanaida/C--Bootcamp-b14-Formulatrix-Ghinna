using BookJournal.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Current Progress")]
        public double CurrentValue { get; set; }

        [Display(Name = "Reading Status")]
        public BookStatus Status { get; set; }

        [Display(Name = "Your Rating (0-10)")]
        [Range(0, 10)]
        public double Rating { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}