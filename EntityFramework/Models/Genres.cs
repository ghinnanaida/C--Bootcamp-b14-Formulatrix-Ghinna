using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    [Table("Genres")]
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}