using Library_Borrowing_Tracker.Client.Models;

namespace Library_Borrowing_Tracker.Client.Services;

public interface ILibraryApiClient
{
    Task<ApiResult<List<BookDto>>> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<BookDto>> GetBookAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResult<BookDto>> CreateBookAsync(BookCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> UpdateBookAsync(BookCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> PatchBookAsync(PatchCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> DeleteBookAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResult<List<MemberDto>>> GetMembersAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<MemberDto>> GetMemberAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResult<MemberDto>> CreateMemberAsync(MemberCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> UpdateMemberAsync(MemberCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> PatchMemberAsync(PatchCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> DeleteMemberAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResult<List<LoanDto>>> GetLoansAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<LoanDto>> GetLoanAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResult<LoanDto>> CreateLoanAsync(LoanCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> UpdateLoanAsync(LoanCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> PatchLoanAsync(PatchCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult> DeleteLoanAsync(int id, CancellationToken cancellationToken = default);
}
