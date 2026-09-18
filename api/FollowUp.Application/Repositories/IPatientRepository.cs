using FollowUp.Application.Entities;

namespace FollowUp.Application.Repositories;

public interface IPatientRepository
{
    Task<bool> ExistsAsync(Country country, DocumentType documentType, string documentNumber);
    Task<bool> ExistsByIdAsync(int id);
    Task<IReadOnlyList<Patient>> GetAllAsync();
    Task AddAsync(Patient patient);
}
