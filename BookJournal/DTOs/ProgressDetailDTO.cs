namespace BookJournal.DTOs
{
    public class ProgressDetailDTO
    {
        public ProgressTrackerDTO Tracker { get; set; } = new();
        public BookDTO Book { get; set; } = new();
    }
}