using FollowUp.Application.Entities;

namespace FollowUp.Application.Services;

public interface IContactService
{
    Task<Contact> RegisterAsync(int patientId, Contact contact);
}
