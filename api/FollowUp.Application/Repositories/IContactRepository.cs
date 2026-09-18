using FollowUp.Application.Entities;

namespace FollowUp.Application.Repositories;

public interface IContactRepository
{
    Task AddAsync(Contact contact);
}
