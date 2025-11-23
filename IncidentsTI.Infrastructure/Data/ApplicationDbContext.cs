using IncidentsTI.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Data;

/// <summary>
/// Database context for the application using Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }
    public DbSet<Incident> Incidents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed roles
        SeedRoles(modelBuilder);

        // Configure Incident relationships
        ConfigureIncidentRelationships(modelBuilder);

        // Entity configurations will be added here as we develop each phase
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private void ConfigureIncidentRelationships(ModelBuilder modelBuilder)
    {
        // Incident -> User (Creator)
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Incident -> Service
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Service)
            .WithMany()
            .HasForeignKey(i => i.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Incident -> User (AssignedTo)
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.AssignedTo)
            .WithMany()
            .HasForeignKey(i => i.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Index for better query performance
        modelBuilder.Entity<Incident>()
            .HasIndex(i => i.TicketNumber)
            .IsUnique();

        modelBuilder.Entity<Incident>()
            .HasIndex(i => i.UserId);

        modelBuilder.Entity<Incident>()
            .HasIndex(i => i.AssignedToId);

        modelBuilder.Entity<Incident>()
            .HasIndex(i => i.Status);
    }

    private void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Student", NormalizedName = "STUDENT" },
            new IdentityRole { Id = "2", Name = "Teacher", NormalizedName = "TEACHER" },
            new IdentityRole { Id = "3", Name = "Administrative", NormalizedName = "ADMINISTRATIVE" },
            new IdentityRole { Id = "4", Name = "Technician", NormalizedName = "TECHNICIAN" },
            new IdentityRole { Id = "5", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
        );
    }
}
