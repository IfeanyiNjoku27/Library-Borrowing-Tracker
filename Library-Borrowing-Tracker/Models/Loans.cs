namespace Library_Borrowing_Tracker.Models
{
    public class Loans
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public Loans(int id, int bookId, int memberId, DateTime borrowDate, DateTime? returnDate, string status)
        {
            Id = id;
            BookId = bookId;
            MemberId = memberId;
            BorrowDate = borrowDate;
            ReturnDate = returnDate;
            Status = status;
        }

        public Loans() { }

    }
}
