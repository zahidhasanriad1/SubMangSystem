using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Assignments;
using AssignFlow.Models.Common;
using AssignFlow.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class AssignmentRepository : Repository<AssignmentItem, Guid>, IAssignmentRepository
{
    public AssignmentRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<AssignmentDto?> GetDetailsAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken) =>
        await ApplyAccess(DbContext.Assignments.AsNoTracking().Where(x => x.Id == id), userId, role)
            .Select(Project(userId)).FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResultDto<AssignmentDto>> GetPagedAsync(AssignmentFilterDto filter, Guid userId, string role, CancellationToken cancellationToken)
    {
        var paging = filter.Paging;
        var query = ApplyAccess(DbContext.Assignments.AsNoTracking(), userId, role);

        if (filter.CourseOfferingId.HasValue)
            query = query.Where(x => x.CourseOfferingId == filter.CourseOfferingId);
        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(paging.Search))
        {
            var term = paging.Search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{term}%") || EF.Functions.ILike(x.CourseOffering.Subject.Code, $"%{term}%"));
        }

        // Count and page are executed by PostgreSQL; no unbounded assignment list is materialized.
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.DeadlineUtc)
            .Skip((paging.SafePage - 1) * paging.SafePageSize).Take(paging.SafePageSize)
            .Select(Project(userId)).ToListAsync(cancellationToken);

        return new PagedResultDto<AssignmentDto>
        {
            Items = items,
            Page = paging.SafePage,
            PageSize = paging.SafePageSize,
            TotalCount = total
        };
    }

    public Task<bool> HasSubmissionsAsync(Guid assignmentId, CancellationToken cancellationToken) =>
        DbContext.Submissions.AnyAsync(x => x.AssignmentId == assignmentId, cancellationToken);

    private static IQueryable<AssignmentItem> ApplyAccess(IQueryable<AssignmentItem> query, Guid userId, string role) => role switch
    {
        // Access is restricted inside the query and defaults to no rows for an unknown role.
        AppRoles.Teacher => query.Where(x => x.CourseOffering.Teachers.Any(t => t.TeacherId == userId)),
        AppRoles.Student => query.Where(x => x.Status == AssignmentStatus.Published && x.CourseOffering.Enrollments.Any(e => e.StudentId == userId)),
        AppRoles.Admin => query,
        _ => query.Where(_ => false)
    };

    private static System.Linq.Expressions.Expression<Func<AssignmentItem, AssignmentDto>> Project(Guid userId) => x => new AssignmentDto
    {
        AssignmentId = x.Id,
        CourseOfferingId = x.CourseOfferingId,
        CourseName = x.CourseOffering.ClassRoom.Name + " - " + x.CourseOffering.ClassRoom.Section,
        SubjectCode = x.CourseOffering.Subject.Code,
        Title = x.Title,
        Description = x.Description,
        DeadlineUtc = x.DeadlineUtc,
        MaximumMarks = x.MaximumMarks,
        AllowResubmission = x.AllowResubmission,
        Status = x.Status,
        PublishedAtUtc = x.PublishedAtUtc,
        CreatedByUserId = x.CreatedById,
        TeacherName = x.CreatedBy.FullName,
        SubmissionCount = x.Submissions.Count,
        HasSubmitted = x.Submissions.Any(s => s.StudentId == userId),
        CreatedAtUtc = x.CreatedAt
    };
}
