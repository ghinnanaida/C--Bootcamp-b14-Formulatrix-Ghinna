using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    /// <summary>
    /// Data Transfer Object for Book information
    /// Simplified version of Book entity for client consumption
    /// </summary>
    public class ProgressTrackerDTO
    {
        /// <summary>
        /// Unique identifier for the progress tracker entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Book identifier associated with this progress entry
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// Current page number the user is on
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of pages in the book
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Date when the progress was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}