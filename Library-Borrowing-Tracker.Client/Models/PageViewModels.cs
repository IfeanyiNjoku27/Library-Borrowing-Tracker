namespace Library_Borrowing_Tracker.Client.Models;

public sealed class BooksPageViewModel
{
    public IReadOnlyList<BookDto> Items { get; init; } = [];
    public BookDto? Selected { get; init; }
    public int? LookupId { get; init; }
    public string? LoadError { get; init; }
    public BookCommand Create { get; init; } = new();
    public BookCommand Update { get; init; } = new();
    public PatchCommand Patch { get; init; } = new();
}

public sealed class MembersPageViewModel
{
    public IReadOnlyList<MemberDto> Items { get; init; } = [];
    public MemberDto? Selected { get; init; }
    public int? LookupId { get; init; }
    public string? LoadError { get; init; }
    public MemberCommand Create { get; init; } = new();
    public MemberCommand Update { get; init; } = new();
    public PatchCommand Patch { get; init; } = new();
}

public sealed class LoansPageViewModel
{
    public IReadOnlyList<LoanDto> Items { get; init; } = [];
    public LoanDto? Selected { get; init; }
    public int? LookupId { get; init; }
    public string? LoadError { get; init; }
    public LoanCommand Create { get; init; } = new();
    public LoanCommand Update { get; init; } = new();
    public PatchCommand Patch { get; init; } = new();
}
