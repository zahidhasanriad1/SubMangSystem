using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.API.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        if (!configuration.GetValue("Seed:Enabled", false)) return;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<AppDbContext>();
        var demoPassword = configuration["Seed:DemoPassword"]
            ?? throw new InvalidOperationException("Seed:DemoPassword is required when demo seeding is enabled.");
        var adminEmail = configuration["Seed:AdminEmail"] ?? "mzhr.riad@gmail.com";
        var adminPassword = configuration["Seed:AdminPassword"]
            ?? throw new InvalidOperationException("Seed:AdminPassword is required when demo seeding is enabled.");

        foreach (var roleName in new[] { AppRoles.Admin, AppRoles.Teacher, AppRoles.Student })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        var admin = await EnsureUserAsync(
            userManager,
            "AssignFlow Administrator",
            adminEmail,
            adminPassword,
            AppRoles.Admin,
            "admin@assignflow.local");
        var teacher = await EnsureUserAsync(userManager, "Demo Teacher", "teacher@assignflow.local", demoPassword, AppRoles.Teacher);
        var student = await EnsureUserAsync(userManager, "Demo Student", "student@assignflow.local", demoPassword, AppRoles.Student);

        if (!await dbContext.ClassRooms.AnyAsync())
        {
            var classRoom = new ClassRoom { Name = "Grade 10", Section = "A", AcademicYear = DateTime.UtcNow.Year };
            var subject = new Subject { Code = "CSE-101", Name = "Computer Science" };
            var course = new CourseOffering { ClassRoom = classRoom, Subject = subject };
            course.Teachers.Add(new CourseTeacher { TeacherId = teacher.Id, AssignedAtUtc = DateTime.UtcNow });
            course.Enrollments.Add(new CourseEnrollment { StudentId = student.Id, EnrolledAtUtc = DateTime.UtcNow });
            course.Assignments.Add(new AssignmentItem
            {
                CreatedById = teacher.Id,
                Title = "Introduction to Algorithms",
                Description = "Explain binary search and analyse its time complexity with one worked example.",
                DeadlineUtc = DateTime.UtcNow.AddDays(7),
                MaximumMarks = 20,
                AllowResubmission = true,
                Status = AssignmentStatus.Published,
                PublishedAtUtc = DateTime.UtcNow
            });
            dbContext.CourseOfferings.Add(course);
        }

        if (!await dbContext.SystemSettings.AnyAsync(x => x.Key == "INSTITUTION_NAME"))
            dbContext.SystemSettings.Add(new SystemSetting { Key = "INSTITUTION_NAME", Value = "AssignFlow Demo College", Description = "Displayed institution name." });

        await dbContext.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string fullName,
        string email,
        string password,
        string role,
        string? legacyEmail = null)
    {
        var user = await userManager.FindByEmailAsync(email);
        var migratedLegacyUser = false;

        // Reuse the original seeded account so an email configuration change never creates a duplicate administrator.
        if (user is null && !string.IsNullOrWhiteSpace(legacyEmail))
        {
            user = await userManager.FindByEmailAsync(legacyEmail);
            if (user is not null)
            {
                user.FullName = fullName;
                user.Email = email;
                user.UserName = email;
                user.EmailConfirmed = true;
                EnsureSucceeded(await userManager.UpdateAsync(user));
                migratedLegacyUser = true;
            }
        }

        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                IsActive = true
            };
            EnsureSucceeded(await userManager.CreateAsync(user, password));
        }
        else if (migratedLegacyUser && !await userManager.CheckPasswordAsync(user, password))
        {
            EnsureSucceeded(await userManager.RemovePasswordAsync(user));
            EnsureSucceeded(await userManager.AddPasswordAsync(user, password));
        }

        if (!await userManager.IsInRoleAsync(user, role))
            EnsureSucceeded(await userManager.AddToRoleAsync(user, role));
        return user;
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(x => x.Description)));
    }
}
