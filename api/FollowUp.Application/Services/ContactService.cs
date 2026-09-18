using FollowUp.Application.Entities;
using FollowUp.Application.Exceptions;
using FollowUp.Application.Repositories;

namespace FollowUp.Application.Services;

public class ContactService : IContactService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IContactRepository _contactRepository;

    public ContactService(IPatientRepository patientRepository, IContactRepository contactRepository)
    {
        _patientRepository = patientRepository;
        _contactRepository = contactRepository;
    }

    public async Task<Contact> RegisterAsync(int patientId, Contact contact)
    {
        var patientExists = await _patientRepository.ExistsByIdAsync(patientId);
        if (!patientExists)
        {
            throw new PatientNotFoundException(patientId);
        }

        contact.PatientId = patientId;
        await _contactRepository.AddAsync(contact);
        return contact;
    }
}
