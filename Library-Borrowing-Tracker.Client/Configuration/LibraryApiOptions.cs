namespace Library_Borrowing_Tracker.Client.Configuration;

public sealed class LibraryApiOptions
{
    public const string SectionName = "LibraryApi";

    public string BaseUrl { get; set; } = "https://localhost:7274/";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiKeyLocation { get; set; } = "Header";

    public string ApiKeyName { get; set; } = "x-apikey";

    public int TimeoutSeconds { get; set; } = 30;
}
