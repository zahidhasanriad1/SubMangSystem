using AssignFlow.DataAccess.Repositories;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Submissions;
using AssignFlow.Services.Services;
using AssignFlow.Tests.Support;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Tests;

public class SubmissionServiceTests
{
    [Fact]
    public async Task SubmitAsync_RejectsSubmissionAfterDeadline()
    {
        await using var db = TestDbContextFactory.Create();
        var assignment = await SeedPublishedAssignmentAsync(db, new DateTime(2026, 8, 13, 8, 0, 0, DateTimeKind.Utc));
        var service = CreateService(db);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SubmitAsync(assignment.Id, new UpsertSubmissionDto { Answer = "Answer" }, StudentId, CancellationToken.None));
    }

    [Fact]
    public async Task SubmitAsync_UpdatesExistingSubmissionWhenResubmissionIsAllowed()
    {
        await using var db = TestDbContextFactory.Create();
        var assignment = await SeedPublishedAssignmentAsync(db, new DateTime(2026, 8, 20, 8, 0, 0, DateTimeKind.Utc));
        var service = CreateService(db);
        await service.SubmitAsync(assignment.Id, new UpsertSubmissionDto { Answer = "First answer" }, StudentId, CancellationToken.None);

        var result = await service.SubmitAsync(assignment.Id, new UpsertSubmissionDto { Answer = "Improved answer" }, StudentId, CancellationToken.None);

        Assert.Equal("Improved answer", result.Answer);
        Assert.Single(db.Submissions);
    }

    [Fact]
    public async Task GradeAsync_RejectsMarksAboveAssignmentMaximum()
    {
        await using var db = TestDbContextFactory.Create();
        var assignment = await SeedPublishedAssignmentAsync(db, new DateTime(2026, 8, 20, 8, 0, 0, DateTimeKind.Utc));
        var service = CreateService(db);
        var submission = await service.SubmitAsync(
            assignment.Id,
            new UpsertSubmissionDto { Answer = "Student answer" },
            StudentId,
            CancellationToken.None);

        await Assert.ThrowsAsync<BadRequestException>(() => service.GradeAsync(
            submission.SubmissionId,
            new GradeSubmissionDto { Marks = 21, Feedback = "Reviewed" },
            TeacherId,
            AppRoles.Teacher,
            CancellationToken.None));
    }

    private static SubmissionService CreateService(AssignFlow.Domain.Database.AppDbContext db) =>
        new(new SubmissionRepository(db), new AssignmentRepository(db), new CourseOfferingRepository(db),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 8, 0, 0, TimeSpan.Zero)));

    private static async Task<AssignmentItem> SeedPublishedAssignmentAsync(AssignFlow.Domain.Database.AppDbContext db, DateTime deadline)
    {
        var teacher = new ApplicationUser { Id = TeacherId, FullName = "Teacher", Email = "teacher@test.local", UserName = "teacher@test.local" };
        var student = new ApplicationUser { Id = StudentId, FullName = "Student", Email = "student@test.local", UserName = "student@test.local" };
        var course = new CourseOffering
        {
            ClassRoom = new ClassRoom { Name = "Grade 10", Section = "A", AcademicYear = 2026 },
            Subject = new Subject { Code = "CSE-101", Name = "Computer Science" }
        };
        course.Teachers.Add(new CourseTeacher { TeacherId = TeacherId, AssignedAtUtc = DateTime.UtcNow });
        course.Enrollments.Add(new CourseEnrollment { StudentId = StudentId, EnrolledAtUtc = DateTime.UtcNow });
        var assignment = new AssignmentItem
        {
            CourseOffering = course,
            CreatedById = TeacherId,
            Title = "Algorithms",
            Description = "Answer",
            DeadlineUtc = deadline,
            MaximumMarks = 20,
            AllowResubmission = true,
            Status = AssignmentStatus.Published
        };
        db.Users.AddRange(teacher, student);
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();
        return assignment;
    }

    private static readonly Guid TeacherId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid StudentId = Guid.Parse("20000000-0000-0000-0000-000000000002");
}
