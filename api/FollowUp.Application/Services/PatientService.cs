using FollowUp.Application.Entities;
using FollowUp.Application.Exceptions;
using FollowUp.Application.Repositories;

namespace FollowUp.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<Patient> RegisterAsync(Patient patient)
    {
        var alreadyExists = await _repository.ExistsAsync(
            patient.Country, patient.DocumentType, patient.DocumentNumber);

        if (alreadyExists)
        {
            throw new DuplicatePatientException(
                patient.Country, patient.DocumentType, patient.DocumentNumber);
        }

        await _repository.AddAsync(patient);
        return patient;
    }
}
