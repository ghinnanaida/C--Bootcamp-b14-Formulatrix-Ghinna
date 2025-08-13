using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    /// <summary>
    /// Data Transfer Object for Book information
    /// Simplified version of Book entity for client consumption
    /// </summary>
    public class BookDTO
    {
        /// <summary>
        /// Unique identifier for the book
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the book
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Author of the book
        /// </summary>
        [Required]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Genre of the book
        /// </summary>
        public string? Genre { get; set; }

        /// <summary>
        /// Publication date of the book
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// ISBN number of the book
        /// </summary>
        public string? ISBN { get; set; }

        /// <summary>
        /// Summary or description of the book
        /// </summary>
        public string? Summary { get; set; }
    }
}