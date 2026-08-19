using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Library_Borrowing_Tracker.Client.Configuration;
using Library_Borrowing_Tracker.Client.Models;
using Microsoft.Extensions.Options;

namespace Library_Borrowing_Tracker.Client.Services;

public sealed class LibraryApiClient : ILibraryApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly LibraryApiOptions _options;

    public LibraryApiClient(HttpClient httpClient, IOptions<LibraryApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public Task<ApiResult<List<BookDto>>> GetBooksAsync(CancellationToken cancellationToken = default) =>
        SendForDataAsync<List<BookDto>>(HttpMethod.Get, "api/books", null, cancellationToken);

    public Task<ApiResult<BookDto>> GetBookAsync(int id, CancellationToken cancellationToken = default) =>
        SendForDataAsync<BookDto>(HttpMethod.Get, $"api/books/{id}", null, cancellationToken);

    public Task<ApiResult<BookDto>> CreateBookAsync(BookCommand command, CancellationToken cancellationToken = default) =>
        SendForDataAsync<BookDto>(HttpMethod.Post, "api/books", new
        {
            command.Title,
            command.Author,
            command.Category,
            command.IsAvailable
        }, cancellationToken);

    public Task<ApiResult> UpdateBookAsync(BookCommand command, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"api/books/{command.Id}", new
        {
            command.Title,
            command.Author,
            command.Category,
            command.IsAvailable
        }, cancellationToken);

    public Task<ApiResult> PatchBookAsync(PatchCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(command.Path, "/title", "/author", "/category", "/isAvailable");
        if (normalizedPath is null)
        {
            return InvalidPatchAsync("Books can patch title, author, category, or isAvailable.");
        }

        object? value = command.Value;
        if (normalizedPath.Equals("/isAvailable", StringComparison.OrdinalIgnoreCase))
        {
            if (!bool.TryParse(command.Value, out var parsed))
            {
                return InvalidPatchAsync("isAvailable must be true or false.");
            }

            value = parsed;
        }

        return SendPatchAsync($"api/books/{command.Id}", normalizedPath, value, cancellationToken);
    }

    public Task<ApiResult> DeleteBookAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"api/books/{id}", null, cancellationToken);

    public Task<ApiResult<List<MemberDto>>> GetMembersAsync(CancellationToken cancellationToken = default) =>
        SendForDataAsync<List<MemberDto>>(HttpMethod.Get, "api/members", null, cancellationToken);

    public Task<ApiResult<MemberDto>> GetMemberAsync(int id, CancellationToken cancellationToken = default) =>
        SendForDataAsync<MemberDto>(HttpMethod.Get, $"api/members/{id}", null, cancellationToken);

    public Task<ApiResult<MemberDto>> CreateMemberAsync(MemberCommand command, CancellationToken cancellationToken = default) =>
        SendForDataAsync<MemberDto>(HttpMethod.Post, "api/members", new
        {
            command.FullName,
            command.Email,
            command.PhoneNumber
        }, cancellationToken);

    public Task<ApiResult> UpdateMemberAsync(MemberCommand command, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"api/members/{command.Id}", new
        {
            command.FullName,
            command.Email,
            command.PhoneNumber
        }, cancellationToken);

    public Task<ApiResult> PatchMemberAsync(PatchCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(command.Path, "/fullName", "/email", "/phoneNumber");
        return normalizedPath is null
            ? InvalidPatchAsync("Members can patch fullName, email, or phoneNumber.")
            : SendPatchAsync($"api/members/{command.Id}", normalizedPath, command.Value, cancellationToken);
    }

    public Task<ApiResult> DeleteMemberAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"api/members/{id}", null, cancellationToken);

    public Task<ApiResult<List<LoanDto>>> GetLoansAsync(CancellationToken cancellationToken = default) =>
        SendForDataAsync<List<LoanDto>>(HttpMethod.Get, "api/loans", null, cancellationToken);

    public Task<ApiResult<LoanDto>> GetLoanAsync(int id, CancellationToken cancellationToken = default) =>
        SendForDataAsync<LoanDto>(HttpMethod.Get, $"api/loans/{id}", null, cancellationToken);

    public Task<ApiResult<LoanDto>> CreateLoanAsync(LoanCommand command, CancellationToken cancellationToken = default) =>
        SendForDataAsync<LoanDto>(HttpMethod.Post, "api/loans", new
        {
            command.BookId,
            command.MemberId,
            command.BorrowDate,
            command.Status
        }, cancellationToken);

    public Task<ApiResult> UpdateLoanAsync(LoanCommand command, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"api/loans/{command.Id}", new
        {
            command.BookId,
            command.MemberId,
            command.BorrowDate,
            command.ReturnDate,
            command.Status
        }, cancellationToken);

    public Task<ApiResult> PatchLoanAsync(PatchCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(
            command.Path,
            "/bookId",
            "/memberId",
            "/borrowDate",
            "/returnDate",
            "/status");

        if (normalizedPath is null)
        {
            return InvalidPatchAsync("Loans can patch bookId, memberId, borrowDate, returnDate, or status.");
        }

        object? value;
        if (normalizedPath.Equals("/bookId", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Equals("/memberId", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(command.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedId))
            {
                return InvalidPatchAsync("bookId and memberId must be whole numbers.");
            }

            value = parsedId;
        }
        else if (normalizedPath.Equals("/borrowDate", StringComparison.OrdinalIgnoreCase)
                 || normalizedPath.Equals("/returnDate", StringComparison.OrdinalIgnoreCase))
        {
            if (normalizedPath.Equals("/returnDate", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(command.Value))
            {
                value = null;
            }
            else if (DateTime.TryParse(command.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedDate))
            {
                value = parsedDate;
            }
            else
            {
                return InvalidPatchAsync("Date values must use an ISO-compatible date or date-time.");
            }
        }
        else
        {
            value = command.Value;
        }

        return SendPatchAsync($"api/loans/{command.Id}", normalizedPath, value, cancellationToken);
    }

    public Task<ApiResult> DeleteLoanAsync(int id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"api/loans/{id}", null, cancellationToken);

    private Task<ApiResult> SendPatchAsync(
        string path,
        string propertyPath,
        object? value,
        CancellationToken cancellationToken)
    {
        var operations = new[]
        {
            new Dictionary<string, object?>
            {
                ["op"] = "replace",
                ["path"] = propertyPath,
                ["value"] = value
            }
        };

        return SendAsync(HttpMethod.Patch, path, operations, cancellationToken, "application/json-patch+json");
    }

    private async Task<ApiResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string path,
        object? payload,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateRequest(method, path, payload);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>(false, response.StatusCode, default, await ReadErrorAsync(response, cancellationToken));
            }

            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return new ApiResult<T>(true, response.StatusCode, data);
        }
        catch (HttpRequestException exception)
        {
            return new ApiResult<T>(false, HttpStatusCode.ServiceUnavailable, default, exception.Message);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            return new ApiResult<T>(false, HttpStatusCode.RequestTimeout, default, exception.Message);
        }
    }

    private async Task<ApiResult> SendAsync(
        HttpMethod method,
        string path,
        object? payload,
        CancellationToken cancellationToken,
        string mediaType = "application/json")
    {
        try
        {
            using var request = CreateRequest(method, path, payload, mediaType);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode
                ? new ApiResult(true, response.StatusCode)
                : new ApiResult(false, response.StatusCode, await ReadErrorAsync(response, cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            return new ApiResult(false, HttpStatusCode.ServiceUnavailable, exception.Message);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            return new ApiResult(false, HttpStatusCode.RequestTimeout, exception.Message);
        }
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string path,
        object? payload,
        string mediaType = "application/json")
    {
        var requestPath = path;
        if (HasApiKey && _options.ApiKeyLocation.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            var separator = requestPath.Contains('?') ? '&' : '?';
            requestPath = $"{requestPath}{separator}{Uri.EscapeDataString(_options.ApiKeyName)}={Uri.EscapeDataString(_options.ApiKey)}";
        }

        var request = new HttpRequestMessage(method, requestPath);
        request.Headers.Accept.ParseAdd("application/json");

        if (HasApiKey && _options.ApiKeyLocation.Equals("Header", StringComparison.OrdinalIgnoreCase))
        {
            request.Headers.TryAddWithoutValidation(_options.ApiKeyName, _options.ApiKey);
        }

        if (payload is not null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload, JsonOptions),
                Encoding.UTF8,
                mediaType);
        }

        return request;
    }

    private bool HasApiKey =>
        !string.IsNullOrWhiteSpace(_options.ApiKey)
        && !string.IsNullOrWhiteSpace(_options.ApiKeyName);

    private static string? NormalizePath(string path, params string[] allowedPaths)
    {
        var candidate = path.Trim();
        if (!candidate.StartsWith('/'))
        {
            candidate = "/" + candidate;
        }

        return allowedPaths.FirstOrDefault(
            allowed => allowed.Equals(candidate, StringComparison.OrdinalIgnoreCase));
    }

    private static Task<ApiResult> InvalidPatchAsync(string message) =>
        Task.FromResult(new ApiResult(false, HttpStatusCode.BadRequest, message));

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();
        }

        return body.Length <= 800 ? body : body[..800] + "…";
    }
}
