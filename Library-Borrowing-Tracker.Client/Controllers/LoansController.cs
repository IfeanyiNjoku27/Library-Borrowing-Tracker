using Library_Borrowing_Tracker.Client.Models;
using Library_Borrowing_Tracker.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Client.Controllers;

public sealed class LoansController(ILibraryApiClient apiClient) : LibraryClientController
{
    [HttpGet]
    public async Task<IActionResult> Index(int? lookupId, CancellationToken cancellationToken)
    {
        var listResult = await apiClient.GetLoansAsync(cancellationToken);
        ApiResult<LoanDto>? selectedResult = null;
        if (lookupId is > 0)
        {
            selectedResult = await apiClient.GetLoanAsync(lookupId.Value, cancellationToken);
        }

        return View(new LoansPageViewModel
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
        [Bind(Prefix = "Create")] LoanCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(LoansController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.CreateLoanAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Created loan #{result.Data?.Id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        [Bind(Prefix = "Update")] LoanCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || command.Id < 1)
        {
            return InvalidForm(nameof(LoansController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.UpdateLoanAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Updated loan #{command.Id} with PUT.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Patch(
        [Bind(Prefix = "Patch")] PatchCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(LoansController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.PatchLoanAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Patched loan #{command.Id}.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id < 1)
        {
            TempData["StatusKind"] = "error";
            TempData["StatusMessage"] = "A valid loan ID is required.";
            return RedirectToAction(nameof(Index));
        }

        var result = await apiClient.DeleteLoanAsync(id, cancellationToken);
        SetStatus(result.IsSuccess, $"Deleted loan #{id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }
}
