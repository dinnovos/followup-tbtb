using FollowUp.Application.Entities;
using FollowUp.Application.Repositories;
using FollowUp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CorrectAsync(Contact contact, ContactCorrection correction)
    {
        _context.Contacts.Update(contact);
        _context.ContactCorrections.Add(correction);
        await _context.SaveChangesAsync();
    }

    // The "consulta con criterio": one query, translated by EF Core into one
    // SQL statement with correlated subqueries, combining Contact +
    // ContactCorrection + Manager (to resolve who corrected, by name).
    public async Task<IReadOnlyList<ContactHistory>> GetHistoryByPatientIdAsync(int patientId)
    {
        return await _context.Contacts
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.ContactDate)
            .Select(c => new ContactHistory
            {
                Contact = c,
                Corrections = _context.ContactCorrections
                    .Where(cc => cc.ContactId == c.Id)
                    .OrderByDescending(cc => cc.CorrectionDate)
                    .Select(cc => new ContactCorrectionSummary
                    {
                        CorrectedByManagerName = _context.Managers
                            .Where(m => m.Id == cc.CorrectedByManagerId)
                            .Select(m => m.Name)
                            .FirstOrDefault() ?? string.Empty,
                        Reason = cc.Reason,
                        CorrectionDate = cc.CorrectionDate,
                        PreviousContactDate = cc.PreviousContactDate,
                        PreviousChannel = cc.PreviousChannel,
                        PreviousResult = cc.PreviousResult,
                        PreviousNotes = cc.PreviousNotes
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
