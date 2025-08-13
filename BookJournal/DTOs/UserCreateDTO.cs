using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }
    }
}