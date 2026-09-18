using FollowUp.Application.Entities;

namespace FollowUp.Application.Repositories;

public interface IPatientRepository
{
    Task<bool> ExistsAsync(Country country, DocumentType documentType, string documentNumber);
    Task AddAsync(Patient patient);
}
