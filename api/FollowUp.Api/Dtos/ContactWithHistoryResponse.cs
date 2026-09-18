using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class ContactWithHistoryResponse
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int ManagerId { get; set; }
    public DateOnly ContactDate { get; set; }
    public Channel Channel { get; set; }
    public ContactResult Result { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<ContactCorrectionResponse> Corrections { get; set; } = new List<ContactCorrectionResponse>();

    public static ContactWithHistoryResponse FromEntity(ContactHistory history) => new()
    {
        Id = history.Contact.Id,
        PatientId = history.Contact.PatientId,
        ManagerId = history.Contact.ManagerId,
        ContactDate = history.Contact.ContactDate,
        Channel = history.Contact.Channel,
        Result = history.Contact.Result,
        Notes = history.Contact.Notes,
        CreatedAt = history.Contact.CreatedAt,
        UpdatedAt = history.Contact.UpdatedAt,
        Corrections = history.Corrections.Select(ContactCorrectionResponse.FromEntity).ToList()
    };
}

public class ContactCorrectionResponse
{
    public string CorrectedByManagerName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CorrectionDate { get; set; }
    public DateOnly PreviousContactDate { get; set; }
    public Channel PreviousChannel { get; set; }
    public ContactResult PreviousResult { get; set; }
    public string? PreviousNotes { get; set; }

    public static ContactCorrectionResponse FromEntity(ContactCorrectionSummary summary) => new()
    {
        CorrectedByManagerName = summary.CorrectedByManagerName,
        Reason = summary.Reason,
        CorrectionDate = summary.CorrectionDate,
        PreviousContactDate = summary.PreviousContactDate,
        PreviousChannel = summary.PreviousChannel,
        PreviousResult = summary.PreviousResult,
        PreviousNotes = summary.PreviousNotes
    };
}
