namespace BookJournal.DTOs
{
    public class BookNoteDTO
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}