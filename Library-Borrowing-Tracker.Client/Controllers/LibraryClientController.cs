using Microsoft.AspNetCore.Mvc;

namespace Library_Borrowing_Tracker.Client.Controllers;

public abstract class LibraryClientController : Controller
{
    protected void SetStatus(bool success, string successMessage, string? error)
    {
        TempData["StatusKind"] = success ? "success" : "error";
        TempData["StatusMessage"] = success ? successMessage : error ?? "The API request failed.";
    }

    protected IActionResult InvalidForm(string controllerName)
    {
        var messages = ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message));

        TempData["StatusKind"] = "error";
        TempData["StatusMessage"] = string.Join(" ", messages.DefaultIfEmpty("Check the submitted values."));
        return RedirectToAction("Index", controllerName);
    }
}
