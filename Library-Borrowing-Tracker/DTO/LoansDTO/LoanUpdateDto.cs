namespace Library_Borrowing_Tracker.DTO.LoansDTO
{
    public class LoanUpdateDto
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
