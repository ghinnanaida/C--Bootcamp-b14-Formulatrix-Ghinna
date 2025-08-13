using BookJournal.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerCreateDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public BookStatus Status { get; set; }
        [Required]
        public BookType BookType { get; set; }
        [Required]
        public ProgressUnit ProgressUnit { get; set; }
        [Required]
        public double TotalValue { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }   
    }
}