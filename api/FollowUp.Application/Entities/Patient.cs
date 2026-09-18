namespace FollowUp.Application.Entities;

public class Patient
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
}
