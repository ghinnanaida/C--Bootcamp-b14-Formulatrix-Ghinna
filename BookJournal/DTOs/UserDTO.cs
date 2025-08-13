using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    /// <summary>
    /// Data Transfer Object for Book information
    /// Simplified version of Book entity for client consumption
    /// </summary>
    public class UserDTO  
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        [Required]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user
        /// </summary>
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Date when the user registered
        /// </summary>
        public DateTime RegistrationDate { get; set; }
    }
}