using FollowUp.Application.Entities;
using FollowUp.Application.Repositories;
using FollowUp.Infrastructure.Data;

namespace FollowUp.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
    }
}
