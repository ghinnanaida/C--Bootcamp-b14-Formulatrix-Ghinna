namespace BookJournal.DTOs
{
    public class DashboardDTO
    {
        public int BooksInProgress { get; set; }
        public int BooksCompleted { get; set; }
        public int BooksOnHold { get; set; }
        public int BooksDropped { get; set; }
        public int BooksToRead { get; set; }
        public IEnumerable<ProgressTrackerDTO> AllBooks { get; set; } = new List<ProgressTrackerDTO>();
        public int BooksCompletedLastWeek { get; set; }
        public double PagesReadLastWeek { get; set; }
        public IEnumerable<ProgressTrackerDTO> TbrBooks { get; set; } = new List<ProgressTrackerDTO>();
        public IEnumerable<ProgressTrackerDTO> InProgressBooks { get; set; } = new List<ProgressTrackerDTO>();
        public IEnumerable<ProgressTrackerDTO> CompletedBooks { get; set; } = new List<ProgressTrackerDTO>();
        public Dictionary<string, int> MonthlyCompletionChartData { get; set; } = new Dictionary<string, int>();
    }
}