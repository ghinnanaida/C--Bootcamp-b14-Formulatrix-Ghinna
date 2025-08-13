using BookJournal.Enumerations;

namespace BookJournal.DTOs
{
    public class ProgressTrackerDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new List<string>();
        public BookStatus Status { get; set; }
        public BookType BookType { get; set; }
        public double Rating { get; set; }
        public double CurrentValue { get; set; }
        public double TotalValue { get; set; }
        public ProgressUnit ProgressUnit { get; set; }
        public double ProgressPercentage { get; set; }
    }
}