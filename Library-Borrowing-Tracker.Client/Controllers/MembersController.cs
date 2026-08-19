using Library_Borrowing_Tracker.Client.Models;
using Library_Borrowing_Tracker.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Client.Controllers;

public sealed class MembersController(ILibraryApiClient apiClient) : LibraryClientController
{
    [HttpGet]
    public async Task<IActionResult> Index(int? lookupId, CancellationToken cancellationToken)
    {
        var listResult = await apiClient.GetMembersAsync(cancellationToken);
        ApiResult<MemberDto>? selectedResult = null;
        if (lookupId is > 0)
        {
            selectedResult = await apiClient.GetMemberAsync(lookupId.Value, cancellationToken);
        }

        return View(new MembersPageViewModel
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
        [Bind(Prefix = "Create")] MemberCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(MembersController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.CreateMemberAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Created member #{result.Data?.Id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        [Bind(Prefix = "Update")] MemberCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || command.Id < 1)
        {
            return InvalidForm(nameof(MembersController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.UpdateMemberAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Updated member #{command.Id} with PUT.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Patch(
        [Bind(Prefix = "Patch")] PatchCommand command,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return InvalidForm(nameof(MembersController).Replace("Controller", string.Empty));
        }

        var result = await apiClient.PatchMemberAsync(command, cancellationToken);
        SetStatus(result.IsSuccess, $"Patched member #{command.Id}.", result.Error);
        return RedirectToAction(nameof(Index), new { lookupId = command.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id < 1)
        {
            TempData["StatusKind"] = "error";
            TempData["StatusMessage"] = "A valid member ID is required.";
            return RedirectToAction(nameof(Index));
        }

        var result = await apiClient.DeleteMemberAsync(id, cancellationToken);
        SetStatus(result.IsSuccess, $"Deleted member #{id}.", result.Error);
        return RedirectToAction(nameof(Index));
    }
}
