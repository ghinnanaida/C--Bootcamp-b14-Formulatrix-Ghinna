using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models
{
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public DateTime ShowTime { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int MovieId { get; set; }    
        public virtual Movie Movie { get; set; } = null!;

        public int StudioId { get; set; }    
        public virtual Studio Studio { get; set; } = null!;
    }
}