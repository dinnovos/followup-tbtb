namespace FollowUp.Application.Entities;

// Not a table -- this is the result shape of the "consulta con criterio"
// (GET /api/patients/{patientId}/contacts): one Contact plus its full
// correction history, with the corrector's name already resolved from
// Manager so the API/UI never has to look it up by id.
public class ContactHistory
{
    public Contact Contact { get; set; } = null!;
    public IReadOnlyList<ContactCorrectionSummary> Corrections { get; set; } = new List<ContactCorrectionSummary>();
}

public class ContactCorrectionSummary
{
    public string CorrectedByManagerName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CorrectionDate { get; set; }
    public DateOnly PreviousContactDate { get; set; }
    public Channel PreviousChannel { get; set; }
    public ContactResult PreviousResult { get; set; }
    public string? PreviousNotes { get; set; }
}
