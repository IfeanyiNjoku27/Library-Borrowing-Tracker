namespace Library_Borrowing_Tracker.DTO
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public bool? isAvailable { get; set; }
    }
}