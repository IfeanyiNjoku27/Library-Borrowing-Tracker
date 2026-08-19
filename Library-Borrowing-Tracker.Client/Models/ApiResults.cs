using System.Net;

namespace Library_Borrowing_Tracker.Client.Models;

public sealed record ApiResult(
    bool IsSuccess,
    HttpStatusCode StatusCode,
    string? Error = null);

public sealed record ApiResult<T>(
    bool IsSuccess,
    HttpStatusCode StatusCode,
    T? Data = default,
    string? Error = null);
