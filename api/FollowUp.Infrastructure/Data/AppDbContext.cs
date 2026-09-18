using FollowUp.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace FollowUp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.ToTable("Manager");
        });

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

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(c => c.Channel)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(c => c.Result)
                .HasConversion<string>()
                .HasMaxLength(30);

            // See 003_create_contact_table.sql: CreatedAt has the same DB-generated
            // default as Patient.CreatedAt.
            entity.Property(c => c.CreatedAt)
                .ValueGeneratedOnAdd();
        });
    }
}
