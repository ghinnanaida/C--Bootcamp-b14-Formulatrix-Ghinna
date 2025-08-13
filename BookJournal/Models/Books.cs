using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookJournal.Models
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Author { get; set; }

        public string? Publisher { get; set; }

        public DateTime? PublishedDate { get; set; }

        public virtual ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
        public virtual ICollection<ProgressTracker> ProgressTrackers { get; set; } = new HashSet<ProgressTracker>();

    }
}