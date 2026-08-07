namespace Library_Borrowing_Tracker.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public bool isAvailable { get; set; }

        public Book(int id, string title, string author, string category, bool isAvailable)
        {
            Id = id;
            Title = title;
            Author = author;
            Category = category;
            this.isAvailable = isAvailable;
        }

        public Book() { }
    }
}
