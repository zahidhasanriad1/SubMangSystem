using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using AssignFlow.Utils.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AssignFlow.DataAccess.Repositories;

public class CourseOfferingRepository : Repository<CourseOffering, Guid>, ICourseOfferingRepository
{
    public CourseOfferingRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<CourseOfferingDto>> GetCourseOfferingsAsync(
        Guid? userId,
        string? role,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.CourseOfferings.AsNoTracking();
        query = role switch
        {
            AppRoles.Teacher when userId.HasValue => query.Where(x => x.Teachers.Any(t => t.TeacherId == userId)),
            AppRoles.Student when userId.HasValue => query.Where(x => x.Enrollments.Any(e => e.StudentId == userId)),
            AppRoles.Admin => query,
            null when !userId.HasValue => query,
            _ => query.Where(_ => false)
        };

        // Only fields required by the client are projected; navigation collections are never materialized.
        return await query
            .OrderByDescending(x => x.ClassRoom.AcademicYear)
            .ThenBy(x => x.Subject.Code)
            .Select(ProjectCourseOffering())
            .ToListAsync(cancellationToken);
    }

    public Task<CourseOfferingDto?> GetCourseOfferingDetailsAsync(
        Guid courseOfferingId,
        CancellationToken cancellationToken = default)
    {
        return DbContext.CourseOfferings.AsNoTracking()
            .Where(x => x.Id == courseOfferingId)
            .Select(ProjectCourseOffering())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid classRoomId,
        Guid subjectId,
        Guid? excludedId,
        CancellationToken cancellationToken = default)
    {
        return DbContext.CourseOfferings.AnyAsync(x =>
            x.ClassRoomId == classRoomId &&
            x.SubjectId == subjectId &&
            x.Id != excludedId,
            cancellationToken);
    }

    public Task<bool> IsTeacherAssignedAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        return DbContext.CourseTeachers.AnyAsync(x =>
            x.CourseOfferingId == courseOfferingId && x.TeacherId == teacherId,
            cancellationToken);
    }

    public Task<bool> IsStudentEnrolledAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return DbContext.CourseEnrollments.AnyAsync(x =>
            x.CourseOfferingId == courseOfferingId && x.StudentId == studentId,
            cancellationToken);
    }

    public async Task<bool> AssignTeacherAsync(CourseTeacher entity, CancellationToken cancellationToken = default)
    {
        await DbContext.CourseTeachers.AddAsync(entity, cancellationToken);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> EnrollStudentAsync(CourseEnrollment entity, CancellationToken cancellationToken = default)
    {
        await DbContext.CourseEnrollments.AddAsync(entity, cancellationToken);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> RemoveTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await DbContext.CourseTeachers
            .Where(x => x.CourseOfferingId == courseOfferingId && x.TeacherId == teacherId)
            .ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> RemoveStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await DbContext.CourseEnrollments
            .Where(x => x.CourseOfferingId == courseOfferingId && x.StudentId == studentId)
            .ExecuteDeleteAsync(cancellationToken) > 0;
    }

    private static Expression<Func<CourseOffering, CourseOfferingDto>> ProjectCourseOffering()
    {
        return x => new CourseOfferingDto
        {
            CourseOfferingId = x.Id,
            ClassRoomId = x.ClassRoomId,
            ClassName = x.ClassRoom.Name,
            Section = x.ClassRoom.Section,
            AcademicYear = x.ClassRoom.AcademicYear,
            SubjectId = x.SubjectId,
            SubjectCode = x.Subject.Code,
            SubjectName = x.Subject.Name,
            IsActive = x.IsActive,
            TeacherCount = x.Teachers.Count,
            StudentCount = x.Enrollments.Count
        };
    }
}
