using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class PatientListItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;

    public static PatientListItemResponse FromEntity(Patient patient) => new()
    {
        Id = patient.Id,
        Name = patient.Name,
        DocumentNumber = patient.DocumentNumber
    };
}
