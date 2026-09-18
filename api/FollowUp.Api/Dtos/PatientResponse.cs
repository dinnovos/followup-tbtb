using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class PatientResponse
{
    public int Id { get; set; }
    public int RegisteredByManagerId { get; set; }
    public Country Country { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string City { get; set; } = string.Empty;
    public DateOnly TreatmentStartDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public static PatientResponse FromEntity(Patient patient) => new()
    {
        Id = patient.Id,
        RegisteredByManagerId = patient.RegisteredByManagerId,
        Country = patient.Country,
        DocumentType = patient.DocumentType,
        DocumentNumber = patient.DocumentNumber,
        Name = patient.Name,
        Phone = patient.Phone,
        Email = patient.Email,
        City = patient.City,
        TreatmentStartDate = patient.TreatmentStartDate,
        CreatedAt = patient.CreatedAt
    };
}
