using AssignFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.Domain.Database;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ClassRoom> ClassRooms { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<CourseOffering> CourseOfferings { get; set; }
    public DbSet<CourseTeacher> CourseTeachers { get; set; }
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    public DbSet<AssignmentItem> Assignments { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity tables use concise domain names while preserving the standard Identity schema.
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Unique and composite indexes enforce business invariants and support the primary list filters.
        builder.Entity<ClassRoom>().HasIndex(x => new { x.Name, x.Section, x.AcademicYear }).IsUnique();
        builder.Entity<Subject>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<CourseOffering>().HasIndex(x => new { x.ClassRoomId, x.SubjectId }).IsUnique();
        builder.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique();
        builder.Entity<AssignmentItem>().HasIndex(x => new { x.CourseOfferingId, x.Status, x.DeadlineUtc });
        builder.Entity<Submission>().HasIndex(x => new { x.AssignmentId, x.StudentId }).IsUnique();
        builder.Entity<Submission>().HasIndex(x => new { x.AssignmentId, x.Status });

        builder.Entity<CourseTeacher>().HasKey(x => new { x.CourseOfferingId, x.TeacherId });
        builder.Entity<CourseEnrollment>().HasKey(x => new { x.CourseOfferingId, x.StudentId });

        builder.Entity<AssignmentItem>().Property(x => x.MaximumMarks).HasPrecision(8, 2);
        builder.Entity<Submission>().Property(x => x.Marks).HasPrecision(8, 2);
        builder.Entity<AssignmentItem>().Property(x => x.Title).HasMaxLength(200);
        builder.Entity<Subject>().Property(x => x.Code).HasMaxLength(30);
        builder.Entity<SystemSetting>().Property(x => x.Key).HasMaxLength(100);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var createdAt = entry.Properties.FirstOrDefault(x => x.Metadata.Name == "CreatedAt");
                if (createdAt is not null) createdAt.CurrentValue = now;

                var updatedAt = entry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdatedAt");
                if (updatedAt is not null) updatedAt.CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                var updatedAt = entry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdatedAt");
                if (updatedAt is not null) updatedAt.CurrentValue = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
