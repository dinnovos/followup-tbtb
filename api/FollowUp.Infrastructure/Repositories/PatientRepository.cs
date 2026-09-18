using FollowUp.Application.Entities;
using FollowUp.Application.Repositories;
using FollowUp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Country country, DocumentType documentType, string documentNumber)
    {
        return await _context.Patients.AnyAsync(p =>
            p.Country == country &&
            p.DocumentType == documentType &&
            p.DocumentNumber == documentNumber);
    }

    public async Task AddAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }
}
