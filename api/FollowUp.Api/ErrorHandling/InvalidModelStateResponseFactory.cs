using System.Text.Json;
using FollowUp.Application.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FollowUp.Api.ErrorHandling;

// Replaces the default [ApiController] 400 response for two problems found by
// testing the UI directly (not just the happy path):
//
// 1. Every field-level error key came back PascalCase ("Name", "Email"),
//    while every Angular form reads fieldErrors in camelCase ("name",
//    "email") -- so no field-specific validation message had ever reached a
//    user, in any form, for any DataAnnotations failure.
// 2. Sending a value that doesn't match an enum (e.g. tampering with a
//    <select>'s value in devtools to send "Colombiax" as Country) fails JSON
//    deserialization before the DTO is even built, producing a raw .NET
//    message ("could not be converted to System.Nullable`1[...]") instead of
//    a readable one -- and a redundant "request field is required" alongside it.
public static class InvalidModelStateResponseFactory
{
    private static readonly Dictionary<string, string> EnumValidValues = new()
    {
        [typeof(Country).FullName!] = string.Join(", ", Enum.GetNames<Country>()),
        [typeof(DocumentType).FullName!] = string.Join(", ", Enum.GetNames<DocumentType>()),
        [typeof(Channel).FullName!] = string.Join(", ", Enum.GetNames<Channel>()),
        [typeof(ContactResult).FullName!] = string.Join(", ", Enum.GetNames<ContactResult>())
    };

    public static IActionResult Create(ActionContext context)
    {
        var fieldEntries = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .Select(entry => (Key: NormalizeKey(entry.Key), entry.Value!.Errors))
            .ToList();

        // "request" only shows up alongside a more specific per-field key
        // when JSON binding failed entirely (case 2 above) -- the specific
        // one already explains what's wrong, so this one is just noise.
        var hasSpecificFieldError = fieldEntries.Any(e => e.Key != "request");
        if (hasSpecificFieldError)
        {
            fieldEntries = fieldEntries.Where(e => e.Key != "request").ToList();
        }

        var errors = fieldEntries.ToDictionary(
            e => e.Key,
            e => e.Errors.Select(err => Humanize(err.ErrorMessage)).ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        return new BadRequestObjectResult(problemDetails);
    }

    private static string NormalizeKey(string key)
    {
        // System.Text.Json binding failures key their ModelState entry as a
        // JSON path ("$.country"); DataAnnotations key it as the plain C#
        // property name ("Name"). Both need to end up camelCase so they
        // match what Angular's fieldErrors[...] lookups expect.
        var fieldName = key.TrimStart('$', '.');
        return fieldName.Length == 0
            ? fieldName
            : JsonNamingPolicy.CamelCase.ConvertName(fieldName);
    }

    private static string Humanize(string message)
    {
        foreach (var (typeFullName, validValues) in EnumValidValues)
        {
            if (message.Contains(typeFullName))
            {
                return $"Must be one of: {validValues}.";
            }
        }

        // Same failure shape as an invalid enum (JSON deserialization fails
        // before the DTO exists), for the other user-editable input type in
        // these DTOs: DateOnly only accepts the ISO 8601 "yyyy-MM-dd" shape
        // that <input type="date"> already sends -- a different format (or
        // hand-crafted JSON) fails the same raw, unreadable way.
        if (message.Contains(nameof(DateOnly)))
        {
            return "Must be a valid date in yyyy-MM-dd format.";
        }

        return message;
    }
}
