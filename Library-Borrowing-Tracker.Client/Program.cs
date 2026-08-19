using Library_Borrowing_Tracker.Client.Configuration;
using Library_Borrowing_Tracker.Client.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddOptions<LibraryApiOptions>()
    .Bind(builder.Configuration.GetSection(LibraryApiOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
        "LibraryApi:BaseUrl must be an absolute HTTP or HTTPS URL.")
    .Validate(
        options => options.ApiKeyLocation.Equals("Header", StringComparison.OrdinalIgnoreCase)
            || options.ApiKeyLocation.Equals("Query", StringComparison.OrdinalIgnoreCase),
        "LibraryApi:ApiKeyLocation must be Header or Query.")
    .ValidateOnStart();

builder.Services.AddHttpClient<ILibraryApiClient, LibraryApiClient>((services, client) =>
{
    var options = services.GetRequiredService<IOptions<LibraryApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/", UriKind.Absolute);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Books");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();

public partial class Program
{
}
