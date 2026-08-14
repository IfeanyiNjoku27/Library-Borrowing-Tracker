namespace Library_Borrowing_Tracker.DTO.BookDTO
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool isAvailable { get; set; } = true;
    }
}