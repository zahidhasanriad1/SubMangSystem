using AssignFlow.DataAccess.Repositories;
using AssignFlow.Domain.Entities;
using AssignFlow.Services.Services;
using AssignFlow.Tests.Support;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Tests;

public class AcademicDeletionTests
{
    [Fact]
    public async Task DeleteClassRoomAsync_RejectsClassWithCourseOffering()
    {
        await using var db = TestDbContextFactory.Create();
        var classRoom = new ClassRoom { Name = "Grade 10", Section = "A", AcademicYear = 2026 };
        db.CourseOfferings.Add(new CourseOffering
        {
            ClassRoom = classRoom,
            Subject = new Subject { Code = "CSE-101", Name = "Computer Science" }
        });
        await db.SaveChangesAsync();
        var service = new ClassRoomService(new ClassRoomRepository(db));

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.DeleteClassRoomAsync(classRoom.Id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteSubjectAsync_DeletesUnlinkedSubject()
    {
        await using var db = TestDbContextFactory.Create();
        var subject = new Subject { Code = "ENG-101", Name = "English" };
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();
        var service = new SubjectService(new SubjectRepository(db));

        var result = await service.DeleteSubjectAsync(subject.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Subjects);
    }

    [Fact]
    public async Task DeleteCourseOfferingAsync_RejectsCourseWithAssignment()
    {
        await using var db = TestDbContextFactory.Create();
        var teacher = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = "Teacher",
            Email = "teacher@test.local",
            UserName = "teacher@test.local"
        };
        var course = new CourseOffering
        {
            ClassRoom = new ClassRoom { Name = "Grade 9", Section = "B", AcademicYear = 2026 },
            Subject = new Subject { Code = "MAT-101", Name = "Mathematics" }
        };
        course.Assignments.Add(new AssignmentItem
        {
            CreatedBy = teacher,
            Title = "Algebra",
            Description = "Solve the equations.",
            DeadlineUtc = DateTime.UtcNow.AddDays(7),
            MaximumMarks = 20
        });
        db.CourseOfferings.Add(course);
        await db.SaveChangesAsync();
        var service = new CourseOfferingService(
            new CourseOfferingRepository(db), null!, null!, null!, TimeProvider.System);

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.DeleteCourseOfferingAsync(course.Id, CancellationToken.None));
    }
}
