using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Dashboard;
using AssignFlow.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _dbContext;

    public DashboardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        // Each role-specific projection is translated into one SQL statement with scalar subqueries.
        // This avoids six sequential database round trips for a single dashboard request.
        var summary = role switch
        {
            AppRoles.Admin => await GetAdminSummaryAsync(userId, cancellationToken),
            AppRoles.Teacher => await GetTeacherSummaryAsync(userId, cancellationToken),
            AppRoles.Student => await GetStudentSummaryAsync(userId, cancellationToken),
            _ => null
        };

        return summary ?? new DashboardSummaryDto { };
    }

    private Task<DashboardSummaryDto?> GetAdminSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().Where(x => x.Id == userId)
            .Select(_ => new DashboardSummaryDto
            {
                Users = _dbContext.Users.Count(),
                Courses = _dbContext.CourseOfferings.Count(),
                Assignments = _dbContext.Assignments.Count(),
                PublishedAssignments = _dbContext.Assignments.Count(x => x.Status == AssignmentStatus.Published),
                Submissions = _dbContext.Submissions.Count(),
                PendingReviews = _dbContext.Submissions.Count(x =>
                    x.Status == SubmissionStatus.Submitted || x.Status == SubmissionStatus.UnderReview)
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    private Task<DashboardSummaryDto?> GetTeacherSummaryAsync(Guid teacherId, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().Where(x => x.Id == teacherId)
            .Select(_ => new DashboardSummaryDto
            {
                Courses = _dbContext.CourseOfferings.Count(x => x.Teachers.Any(t => t.TeacherId == teacherId)),
                Assignments = _dbContext.Assignments.Count(x => x.CourseOffering.Teachers.Any(t => t.TeacherId == teacherId)),
                PublishedAssignments = _dbContext.Assignments.Count(x =>
                    x.Status == AssignmentStatus.Published && x.CourseOffering.Teachers.Any(t => t.TeacherId == teacherId)),
                Submissions = _dbContext.Submissions.Count(x =>
                    x.Assignment.CourseOffering.Teachers.Any(t => t.TeacherId == teacherId)),
                PendingReviews = _dbContext.Submissions.Count(x =>
                    x.Assignment.CourseOffering.Teachers.Any(t => t.TeacherId == teacherId) &&
                    (x.Status == SubmissionStatus.Submitted || x.Status == SubmissionStatus.UnderReview))
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    private Task<DashboardSummaryDto?> GetStudentSummaryAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().Where(x => x.Id == studentId)
            .Select(_ => new DashboardSummaryDto
            {
                Courses = _dbContext.CourseOfferings.Count(x => x.Enrollments.Any(e => e.StudentId == studentId)),
                Assignments = _dbContext.Assignments.Count(x =>
                    x.Status == AssignmentStatus.Published && x.CourseOffering.Enrollments.Any(e => e.StudentId == studentId)),
                PublishedAssignments = _dbContext.Assignments.Count(x =>
                    x.Status == AssignmentStatus.Published && x.CourseOffering.Enrollments.Any(e => e.StudentId == studentId)),
                Submissions = _dbContext.Submissions.Count(x => x.StudentId == studentId),
                PendingReviews = _dbContext.Submissions.Count(x =>
                    x.StudentId == studentId &&
                    (x.Status == SubmissionStatus.Submitted || x.Status == SubmissionStatus.UnderReview))
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
