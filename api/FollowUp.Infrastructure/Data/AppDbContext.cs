using FollowUp.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");

            entity.Property(p => p.Country)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // CreatedAt is filled by the database default (SYSUTCDATETIME(), see
            // 002_create_patient_table.sql) -- EF must never send its own value.
            entity.Property(p => p.CreatedAt)
                .ValueGeneratedOnAdd();
        });
    }
}
