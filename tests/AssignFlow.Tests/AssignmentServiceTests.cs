using AssignFlow.DataAccess.Repositories;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Assignments;
using AssignFlow.Services.Services;
using AssignFlow.Tests.Support;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Tests;

public class AssignmentServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsTeacherWhoIsNotAssignedToCourse()
    {
        await using var db = TestDbContextFactory.Create();
        var course = await SeedCourseAsync(db, assignTeacher: false);
        var service = new AssignmentService(new AssignmentRepository(db), new CourseOfferingRepository(db),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 8, 0, 0, TimeSpan.Zero)));
        var request = CreatePublishedRequest(course.Id);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.CreateAsync(request, TestIds.TeacherId, AppRoles.Teacher, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_AllowsAssignedTeacherAndPublishesAssignment()
    {
        await using var db = TestDbContextFactory.Create();
        var course = await SeedCourseAsync(db, assignTeacher: true);
        var service = new AssignmentService(new AssignmentRepository(db), new CourseOfferingRepository(db),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 8, 0, 0, TimeSpan.Zero)));
        var request = CreatePublishedRequest(course.Id);

        var result = await service.CreateAsync(request, TestIds.TeacherId, AppRoles.Teacher, CancellationToken.None);

        Assert.Equal(AssignmentStatus.Published, result.Status);
        Assert.Equal(20, result.MaximumMarks);
    }

    [Fact]
    public async Task CreateAsync_RejectsAdminBecauseAssignmentAuthoringBelongsToTeacher()
    {
        await using var db = TestDbContextFactory.Create();
        var course = await SeedCourseAsync(db, assignTeacher: true);
        var service = new AssignmentService(new AssignmentRepository(db), new CourseOfferingRepository(db),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 8, 0, 0, TimeSpan.Zero)));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.CreateAsync(CreatePublishedRequest(course.Id), TestIds.TeacherId, AppRoles.Admin, CancellationToken.None));
    }

    private static async Task<CourseOffering> SeedCourseAsync(AssignFlow.Domain.Database.AppDbContext db, bool assignTeacher)
    {
        var teacher = new ApplicationUser { Id = TestIds.TeacherId, FullName = "Teacher", Email = "teacher@test.local", UserName = "teacher@test.local" };
        var course = new CourseOffering
        {
            ClassRoom = new ClassRoom { Name = "Grade 10", Section = "A", AcademicYear = 2026 },
            Subject = new Subject { Code = "CSE-101", Name = "Computer Science" }
        };
        db.Users.Add(teacher);
        db.CourseOfferings.Add(course);
        if (assignTeacher) course.Teachers.Add(new CourseTeacher { TeacherId = teacher.Id, AssignedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();
        return course;
    }

    private static CreateAssignmentDto CreatePublishedRequest(Guid courseOfferingId)
    {
        return new CreateAssignmentDto
        {
            CourseOfferingId = courseOfferingId,
            Title = "Algorithms",
            Description = "Describe binary search.",
            DeadlineUtc = new DateTime(2026, 8, 20, 8, 0, 0, DateTimeKind.Utc),
            MaximumMarks = 20,
            AllowResubmission = true,
            Status = AssignmentStatus.Published
        };
    }

    private static class TestIds
    {
        public static readonly Guid TeacherId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    }
}
