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
    public DbSet<IncidentHistory> IncidentHistories { get; set; }
    public DbSet<IncidentComment> IncidentComments { get; set; }
    public DbSet<EscalationLevel> EscalationLevels { get; set; }
    public DbSet<IncidentEscalation> IncidentEscalations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed roles
        SeedRoles(modelBuilder);

        // Configure Incident relationships
        ConfigureIncidentRelationships(modelBuilder);
        
        // Configure IncidentHistory relationships
        ConfigureIncidentHistoryRelationships(modelBuilder);
        
        // Configure IncidentComment relationships
        ConfigureIncidentCommentRelationships(modelBuilder);
        
        // Configure EscalationLevel relationships
        ConfigureEscalationLevelRelationships(modelBuilder);
        
        // Configure IncidentEscalation relationships
        ConfigureIncidentEscalationRelationships(modelBuilder);

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

        // Incident -> EscalationLevel (CurrentLevel)
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.CurrentEscalationLevel)
            .WithMany(e => e.Incidents)
            .HasForeignKey(i => i.CurrentEscalationLevelId)
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

    private void ConfigureIncidentHistoryRelationships(ModelBuilder modelBuilder)
    {
        // IncidentHistory -> Incident
        modelBuilder.Entity<IncidentHistory>()
            .HasOne(h => h.Incident)
            .WithMany(i => i.History)
            .HasForeignKey(h => h.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // IncidentHistory -> User
        modelBuilder.Entity<IncidentHistory>()
            .HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better query performance
        modelBuilder.Entity<IncidentHistory>()
            .HasIndex(h => h.IncidentId);

        modelBuilder.Entity<IncidentHistory>()
            .HasIndex(h => h.Timestamp);
    }

    private void ConfigureIncidentCommentRelationships(ModelBuilder modelBuilder)
    {
        // IncidentComment -> Incident
        modelBuilder.Entity<IncidentComment>()
            .HasOne(c => c.Incident)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // IncidentComment -> User
        modelBuilder.Entity<IncidentComment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better query performance
        modelBuilder.Entity<IncidentComment>()
            .HasIndex(c => c.IncidentId);

        modelBuilder.Entity<IncidentComment>()
            .HasIndex(c => c.CreatedAt);
    }

    private void ConfigureEscalationLevelRelationships(ModelBuilder modelBuilder)
    {
        // Index for ordering
        modelBuilder.Entity<EscalationLevel>()
            .HasIndex(e => e.Order)
            .IsUnique();

        modelBuilder.Entity<EscalationLevel>()
            .HasIndex(e => e.IsActive);
    }

    private void ConfigureIncidentEscalationRelationships(ModelBuilder modelBuilder)
    {
        // IncidentEscalation -> Incident
        modelBuilder.Entity<IncidentEscalation>()
            .HasOne(e => e.Incident)
            .WithMany(i => i.Escalations)
            .HasForeignKey(e => e.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // IncidentEscalation -> FromUser
        modelBuilder.Entity<IncidentEscalation>()
            .HasOne(e => e.FromUser)
            .WithMany()
            .HasForeignKey(e => e.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // IncidentEscalation -> ToUser (nullable)
        modelBuilder.Entity<IncidentEscalation>()
            .HasOne(e => e.ToUser)
            .WithMany()
            .HasForeignKey(e => e.ToUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // IncidentEscalation -> FromLevel (nullable)
        modelBuilder.Entity<IncidentEscalation>()
            .HasOne(e => e.FromLevel)
            .WithMany(l => l.EscalationsFrom)
            .HasForeignKey(e => e.FromLevelId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // IncidentEscalation -> ToLevel
        modelBuilder.Entity<IncidentEscalation>()
            .HasOne(e => e.ToLevel)
            .WithMany(l => l.EscalationsTo)
            .HasForeignKey(e => e.ToLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better query performance
        modelBuilder.Entity<IncidentEscalation>()
            .HasIndex(e => e.IncidentId);

        modelBuilder.Entity<IncidentEscalation>()
            .HasIndex(e => e.EscalatedAt);
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
