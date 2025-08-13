namespace BookJournal.DTOs
{
    public class DashboardDTO
    {
        public int BooksInProgress { get; set; }
        public int BooksCompleted { get; set; }
        public int BooksOnHold { get; set; }
        public int BooksDropped { get; set; }
        public IEnumerable<ProgressTrackerDTO> AllBooks { get; set; } = new List<ProgressTrackerDTO>();
    }
}