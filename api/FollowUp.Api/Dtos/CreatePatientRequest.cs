using System.ComponentModel.DataAnnotations;
using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class CreatePatientRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "RegisteredByManagerId is required.")]
    public int RegisteredByManagerId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public Country? Country { get; set; }

    public DocumentType? DocumentType { get; set; }

    [Required, MaxLength(20)]
    public string DocumentNumber { get; set; } = string.Empty;

    // Digits only, optional leading "+" for a country code, 7-15 digits total --
    // loose enough to cover Colombia/Peru/Ecuador with or without a country
    // code, without inventing a per-country format the PRD never specified
    // (hallazgo: "no phone format defined for a 3-country program").
    [Required, RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Phone must contain only digits (7 to 15), with an optional leading '+'.")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public DateOnly? TreatmentStartDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Country is null)
        {
            yield return new ValidationResult("Country is required.", new[] { nameof(Country) });
        }

        if (DocumentType is null)
        {
            yield return new ValidationResult("DocumentType is required.", new[] { nameof(DocumentType) });
        }

        if (TreatmentStartDate is null)
        {
            yield return new ValidationResult("TreatmentStartDate is required.", new[] { nameof(TreatmentStartDate) });
        }
    }
}
