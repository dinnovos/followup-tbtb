using FollowUp.Application.Entities;

namespace FollowUp.Application.Services;

public interface IPatientService
{
    Task<Patient> RegisterAsync(Patient patient);
}
