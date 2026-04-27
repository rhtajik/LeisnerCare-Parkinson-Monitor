using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Symptom> Symptoms => Set<Symptom>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationLog> MedicationLogs => Set<MedicationLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Patient konfiguration
        builder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CprNumber).IsUnique();
            entity.Property(e => e.CprNumber).HasMaxLength(10);

            entity.HasOne(e => e.User)
                  .WithOne()
                  .HasForeignKey<Patient>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Symptom konfiguration
        builder.Entity<Symptom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Medication konfiguration
        builder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Dosage).HasMaxLength(50);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // MedicationLog konfiguration
        builder.Entity<MedicationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Medication)
                  .WithMany(m => m.Logs)
                  .HasForeignKey(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}