using System.ComponentModel.DataAnnotations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerDTO
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}