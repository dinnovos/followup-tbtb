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

    public async Task<Contact> CorrectAsync(
        int contactId,
        int correctedByManagerId,
        string reason,
        DateOnly? contactDate,
        Channel? channel,
        ContactResult? result,
        string? notes)
    {
        if (contactDate is null && channel is null && result is null && notes is null)
        {
            throw new ArgumentException("At least one field to correct must be provided.");
        }

        var contact = await _contactRepository.GetByIdAsync(contactId);
        if (contact is null)
        {
            throw new ContactNotFoundException(contactId);
        }

        var correction = new ContactCorrection
        {
            ContactId = contact.Id,
            PreviousContactDate = contact.ContactDate,
            PreviousChannel = contact.Channel,
            PreviousResult = contact.Result,
            PreviousNotes = contact.Notes,
            CorrectedByManagerId = correctedByManagerId,
            Reason = reason
        };

        if (contactDate is not null)
        {
            contact.ContactDate = contactDate.Value;
        }

        if (channel is not null)
        {
            contact.Channel = channel.Value;
        }

        if (result is not null)
        {
            contact.Result = result.Value;
        }

        if (notes is not null)
        {
            contact.Notes = notes;
        }

        contact.UpdatedAt = DateTime.UtcNow;

        await _contactRepository.CorrectAsync(contact, correction);
        return contact;
    }
}
