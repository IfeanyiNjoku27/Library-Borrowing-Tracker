using System.Net;
using System.Text;
using Library_Borrowing_Tracker.Client.Configuration;
using Library_Borrowing_Tracker.Client.Models;
using Library_Borrowing_Tracker.Client.Services;
using Microsoft.Extensions.Options;

namespace Library_Borrowing_Tracker.Client.Tests;

public sealed class LibraryApiClientTests
{
    [Fact]
    public async Task AllEighteenActionsUseExpectedMethodsRoutesAndApiKeyHeader()
    {
        var (client, handler) = CreateClient(new LibraryApiOptions
        {
            BaseUrl = "https://gateway.example/",
            ApiKey = "test-key",
            ApiKeyLocation = "Header",
            ApiKeyName = "x-api-key"
        });

        var book = new BookCommand { Id = 7, Title = "Dune", Author = "Frank Herbert", Category = "Science fiction", IsAvailable = true };
        var member = new MemberCommand { Id = 8, FullName = "Ada Lovelace", Email = "ada@example.test", PhoneNumber = "555-0100" };
        var loan = new LoanCommand { Id = 9, BookId = 7, MemberId = 8, BorrowDate = new DateTime(2026, 8, 18), Status = "Active" };

        await client.GetBooksAsync();
        await client.GetBookAsync(7);
        await client.CreateBookAsync(book);
        await client.UpdateBookAsync(book);
        await client.PatchBookAsync(new PatchCommand { Id = 7, Path = "/title", Value = "Dune Messiah" });
        await client.DeleteBookAsync(7);

        await client.GetMembersAsync();
        await client.GetMemberAsync(8);
        await client.CreateMemberAsync(member);
        await client.UpdateMemberAsync(member);
        await client.PatchMemberAsync(new PatchCommand { Id = 8, Path = "/email", Value = "new@example.test" });
        await client.DeleteMemberAsync(8);

        await client.GetLoansAsync();
        await client.GetLoanAsync(9);
        await client.CreateLoanAsync(loan);
        await client.UpdateLoanAsync(loan);
        await client.PatchLoanAsync(new PatchCommand { Id = 9, Path = "/status", Value = "Returned" });
        await client.DeleteLoanAsync(9);

        var expected = new (HttpMethod Method, string Path)[]
        {
            (HttpMethod.Get, "/api/books"),
            (HttpMethod.Get, "/api/books/7"),
            (HttpMethod.Post, "/api/books"),
            (HttpMethod.Put, "/api/books/7"),
            (HttpMethod.Patch, "/api/books/7"),
            (HttpMethod.Delete, "/api/books/7"),
            (HttpMethod.Get, "/api/members"),
            (HttpMethod.Get, "/api/members/8"),
            (HttpMethod.Post, "/api/members"),
            (HttpMethod.Put, "/api/members/8"),
            (HttpMethod.Patch, "/api/members/8"),
            (HttpMethod.Delete, "/api/members/8"),
            (HttpMethod.Get, "/api/loans"),
            (HttpMethod.Get, "/api/loans/9"),
            (HttpMethod.Post, "/api/loans"),
            (HttpMethod.Put, "/api/loans/9"),
            (HttpMethod.Patch, "/api/loans/9"),
            (HttpMethod.Delete, "/api/loans/9")
        };

        Assert.Equal(18, handler.Requests.Count);
        Assert.Equal(expected, handler.Requests.Select(request => (request.Method, request.Path)));
        Assert.All(handler.Requests, request => Assert.Equal("test-key", request.ApiKey));
        Assert.All(
            handler.Requests.Where(request => request.Method == HttpMethod.Patch),
            request => Assert.Equal("application/json-patch+json", request.ContentType));
    }

    [Fact]
    public async Task ApiKeyCanBeSentAsConfiguredQueryParameter()
    {
        var (client, handler) = CreateClient(new LibraryApiOptions
        {
            BaseUrl = "https://gateway.example/",
            ApiKey = "abc+123",
            ApiKeyLocation = "Query",
            ApiKeyName = "apikey"
        });

        await client.GetBooksAsync();

        var request = Assert.Single(handler.Requests);
        Assert.Equal("/api/books?apikey=abc%2B123", request.Path);
        Assert.Null(request.ApiKey);
    }

    [Fact]
    public async Task EmptyApiKeyDoesNotAddCredentialToRequest()
    {
        var (client, handler) = CreateClient(new LibraryApiOptions
        {
            BaseUrl = "https://localhost:7274/",
            ApiKey = string.Empty,
            ApiKeyLocation = "Header",
            ApiKeyName = "x-api-key"
        });

        await client.GetBooksAsync();

        var request = Assert.Single(handler.Requests);
        Assert.Equal("/api/books", request.Path);
        Assert.Null(request.ApiKey);
    }

    [Fact]
    public async Task PatchSerializesBooleanAndNumericValuesWithJsonTypes()
    {
        var (client, handler) = CreateClient(new LibraryApiOptions { BaseUrl = "https://gateway.example/" });

        await client.PatchBookAsync(new PatchCommand { Id = 2, Path = "/isAvailable", Value = "false" });
        await client.PatchLoanAsync(new PatchCommand { Id = 3, Path = "/bookId", Value = "42" });

        Assert.Contains("\"value\":false", handler.Requests[0].Body);
        Assert.Contains("\"value\":42", handler.Requests[1].Body);
    }

    private static (LibraryApiClient Client, RecordingHandler Handler) CreateClient(LibraryApiOptions options)
    {
        var handler = new RecordingHandler(options.ApiKeyName);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.BaseUrl)
        };

        return (new LibraryApiClient(httpClient, Options.Create(options)), handler);
    }

    private sealed class RecordingHandler(string apiKeyHeaderName) : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            var apiKey = request.Headers.TryGetValues(apiKeyHeaderName, out var values)
                ? values.Single()
                : null;

            Requests.Add(new RecordedRequest(
                request.Method,
                request.RequestUri?.PathAndQuery ?? string.Empty,
                apiKey,
                request.Content?.Headers.ContentType?.MediaType,
                body));

            return CreateResponse(request);
        }

        private static HttpResponseMessage CreateResponse(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get && request.RequestUri?.AbsolutePath.EndsWith("s", StringComparison.Ordinal) == true)
            {
                return Json(HttpStatusCode.OK, "[]");
            }

            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Post)
            {
                var path = request.RequestUri?.AbsolutePath ?? string.Empty;
                if (path.Contains("books", StringComparison.Ordinal))
                {
                    return Json(request.Method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK,
                        "{\"id\":7,\"title\":\"Dune\",\"author\":\"Frank Herbert\",\"category\":\"Science fiction\",\"isAvailable\":true}");
                }

                if (path.Contains("members", StringComparison.Ordinal))
                {
                    return Json(request.Method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK,
                        "{\"id\":8,\"fullName\":\"Ada Lovelace\",\"email\":\"ada@example.test\",\"phoneNumber\":\"555-0100\"}");
                }

                return Json(request.Method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK,
                    "{\"id\":9,\"bookId\":7,\"memberId\":8,\"borrowDate\":\"2026-08-18T00:00:00\",\"returnDate\":null,\"status\":\"Active\"}");
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private static HttpResponseMessage Json(HttpStatusCode statusCode, string json) =>
            new(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
    }

    private sealed record RecordedRequest(
        HttpMethod Method,
        string Path,
        string? ApiKey,
        string? ContentType,
        string Body);
}
