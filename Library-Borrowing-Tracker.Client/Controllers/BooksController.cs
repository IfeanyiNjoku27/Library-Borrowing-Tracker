using Library_Borrowing_Tracker.Client.Models;
using Library_Borrowing_Tracker.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Client.Controllers;

public sealed class BooksController(ILibraryApiClient apiClient) : LibraryClientController
{
    [HttpGet]
    public async Task<IActionResult> Index(int? lookupId, CancellationToken cancellationToken)
    {
        var listResult = await apiClient.GetBooksAsync(cancellationToken);
        ApiResult<BookDto>? selectedResult = null;
        if (lookupId is > 0)
        {
            selectedResult = await apiClient.GetBookAsync(lookupId.Value, cancellationToken);
        }

        return View(new BooksPageViewModel
        {
            Items = listResult.Data ?? [],
            LookupId = lookupId,
            Selected = selectedResult?.Data,
            LoadError = !listResult.IsSuccess
                ? listResult.Error
                : selectedResult is { IsSuccess: false }
                    ? selectedResult.Error
                    : null
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(Prefix = "Create")] BookCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(BooksController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.CreateBookAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Created book #{result.Data?.Id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        [Bind(Prefix = "Update")] BookCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || command.Id < 1)
        {
            return InvalidForm(nameof(BooksController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.UpdateBookAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Updated book #{command.Id} with PUT.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Patch(
        [Bind(Prefix = "Patch")] PatchCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(BooksController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.PatchBookAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Patched book #{command.Id}.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id < 1)
        {
            TempData["StatusKind"] = "error";
            TempData["StatusMessage"] = "A valid book ID is required.";
            return RedirectToAction(nameof(Index));
        }

        var result = await apiClient.DeleteBookAsync(id, cancellationToken);
        SetStatus(result.IsSuccess, $"Deleted book #{id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }
}
