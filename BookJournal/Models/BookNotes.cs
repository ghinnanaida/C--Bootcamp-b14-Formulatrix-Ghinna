using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookJournal.Models
{
    [Table("BookNotes")]
    public class BookNotes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Note { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProgressTrackerId { get; set; }

        public virtual ProgressTracker ProgressTracker { get; set; } = null!;

    }
}