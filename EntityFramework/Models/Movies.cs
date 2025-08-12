using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    [Table("Movies")]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int GenreId { get; set; }    
        public virtual Genre Genre { get; set; } = null!;

        public virtual ICollection<Schedule> ShowTime { get; set; } = new HashSet<Schedule>();

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Range(0, 10)]
        public double Rating { get; set; }
    }
}