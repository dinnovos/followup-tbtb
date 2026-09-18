namespace FollowUp.Application.Entities;

public class ContactCorrection
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public DateOnly PreviousContactDate { get; set; }
    public Channel PreviousChannel { get; set; }
    public ContactResult PreviousResult { get; set; }
    public string? PreviousNotes { get; set; }
    public int CorrectedByManagerId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CorrectionDate { get; set; }
}
