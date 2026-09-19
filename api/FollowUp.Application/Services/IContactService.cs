using FollowUp.Application.Entities;

namespace FollowUp.Application.Services;

public interface IContactService
{
    Task<Contact> RegisterAsync(int patientId, Contact contact);

    Task<IReadOnlyList<ContactHistory>> GetHistoryByPatientIdAsync(int patientId);

    Task<Contact> CorrectAsync(
        int contactId,
        int correctedByManagerId,
        string reason,
        DateOnly? contactDate,
        Channel? channel,
        ContactResult? result,
        string? notes);
}
