using FollowUp.Application.Entities;
using FollowUp.Application.Repositories;
using FollowUp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Infrastructure.Repositories;

public class ManagerRepository : IManagerRepository
{
    private readonly AppDbContext _context;

    public ManagerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Manager>> GetAllAsync()
    {
        return await _context.Managers
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
