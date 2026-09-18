using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class ContactResponse
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

    public static ContactResponse FromEntity(Contact contact) => new()
    {
        Id = contact.Id,
        PatientId = contact.PatientId,
        ManagerId = contact.ManagerId,
        ContactDate = contact.ContactDate,
        Channel = contact.Channel,
        Result = contact.Result,
        Notes = contact.Notes,
        CreatedAt = contact.CreatedAt,
        UpdatedAt = contact.UpdatedAt
    };
}
