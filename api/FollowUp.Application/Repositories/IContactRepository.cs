using FollowUp.Application.Entities;

namespace FollowUp.Application.Repositories;

public interface IContactRepository
{
    Task AddAsync(Contact contact);
    Task<Contact?> GetByIdAsync(int id);
    Task CorrectAsync(Contact contact, ContactCorrection correction);
    Task<IReadOnlyList<ContactHistory>> GetHistoryByPatientIdAsync(int patientId);
}
