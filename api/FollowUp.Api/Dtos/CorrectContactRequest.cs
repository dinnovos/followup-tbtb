using System.ComponentModel.DataAnnotations;
using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class CorrectContactRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "CorrectedByManagerId is required.")]
    public int CorrectedByManagerId { get; set; }

    [Required, MaxLength(300)]
    public string Reason { get; set; } = string.Empty;

    public DateOnly? ContactDate { get; set; }

    public Channel? Channel { get; set; }

    public ContactResult? Result { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ContactDate is null && Channel is null && Result is null && Notes is null)
        {
            yield return new ValidationResult(
                "At least one field to correct must be provided.",
                new[] { nameof(ContactDate), nameof(Channel), nameof(Result), nameof(Notes) });
        }
    }
}
