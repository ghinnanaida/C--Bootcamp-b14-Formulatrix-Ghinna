using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookJournal.Enumerations;

namespace BookJournal.Models
{
    [Table("ProgressTrackers")]
    public class ProgressTracker
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [Required]
        public required int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;

        [Required]
        public required BookStatus Status { get; set; }
        [Required]
        public required BookType BookType { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public required DateTime LastStatusChangeDate { get; set; }

        [Required]
        public required ProgressUnit ProgressUnit { get; set; }

        [Required]
        public required double TotalValue { get; set; }
        public double CurrentValue { get; set; }
        
        [Range(0, 10)]
        public double Rating { get; set; }

    }
}