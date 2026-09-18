using System.ComponentModel.DataAnnotations;
using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class CreateContactRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "ManagerId is required.")]
    public int ManagerId { get; set; }

    public DateOnly? ContactDate { get; set; }

    public Channel? Channel { get; set; }

    public ContactResult? Result { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ContactDate is null)
        {
            yield return new ValidationResult("ContactDate is required.", new[] { nameof(ContactDate) });
        }

        if (Channel is null)
        {
            yield return new ValidationResult("Channel is required.", new[] { nameof(Channel) });
        }

        if (Result is null)
        {
            yield return new ValidationResult("Result is required.", new[] { nameof(Result) });
        }
    }
}
