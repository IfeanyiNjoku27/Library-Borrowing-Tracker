using System.ComponentModel.DataAnnotations;

namespace Library_Borrowing_Tracker.Client.Models;

public sealed class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public sealed class MemberDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public sealed class LoanDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class BookCommand
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Author { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
}

public sealed class MemberCommand
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(40)]
    public string PhoneNumber { get; set; } = string.Empty;
}

public sealed class LoanCommand
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int BookId { get; set; }

    [Range(1, int.MaxValue)]
    public int MemberId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime BorrowDate { get; set; } = DateTime.Today;

    [DataType(DataType.DateTime)]
    public DateTime? ReturnDate { get; set; }

    [Required, StringLength(40)]
    public string Status { get; set; } = "Active";
}

public sealed class PatchCommand
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    public string Path { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
