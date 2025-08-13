using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    /// <summary>
    /// Data Transfer Object for Book information
    /// Simplified version of Book entity for client consumption
    /// </summary>
    public class GenreDTO
    {
        /// <summary>
        /// Unique identifier for the genre
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the genre
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the genre
        /// </summary>
        public string? Description { get; set; }
    }
}