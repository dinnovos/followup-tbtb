using FollowUp.Application.Entities;

namespace FollowUp.Application.Repositories;

public interface IManagerRepository
{
    Task<IReadOnlyList<Manager>> GetAllAsync();
}
