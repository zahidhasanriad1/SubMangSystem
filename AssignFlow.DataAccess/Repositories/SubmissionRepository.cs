using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Common;
using AssignFlow.Models.Submissions;
using AssignFlow.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class SubmissionRepository : Repository<Submission, Guid>, ISubmissionRepository
{
    public SubmissionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbContext.Submissions.Include(x => x.Assignment).ThenInclude(x => x.CourseOffering)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken) =>
        DbContext.Submissions.Include(x => x.Assignment)
            .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.StudentId == studentId, cancellationToken);

    public async Task<SubmissionDto?> GetDetailsAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken) =>
        await ApplyAccess(DbContext.Submissions.AsNoTracking().Where(x => x.Id == id), userId, role)
            .Select(Project()).FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResultDto<SubmissionDto>> GetPagedAsync(SubmissionFilterDto filter, Guid userId, string role, CancellationToken cancellationToken)
    {
        var paging = filter.Paging;
        var query = ApplyAccess(DbContext.Submissions.AsNoTracking(), userId, role);
        if (filter.AssignmentId.HasValue)
            query = query.Where(x => x.AssignmentId == filter.AssignmentId);
        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(paging.Search))
        {
            var term = paging.Search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Student.FullName, $"%{term}%") || EF.Functions.ILike(x.Assignment.Title, $"%{term}%"));
        }

        // The database performs filtering, ordering, and paging before DTO materialization.
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.SubmittedAtUtc)
            .Skip((paging.SafePage - 1) * paging.SafePageSize).Take(paging.SafePageSize)
            .Select(Project()).ToListAsync(cancellationToken);
        return new PagedResultDto<SubmissionDto>
        {
            Items = items,
            Page = paging.SafePage,
            PageSize = paging.SafePageSize,
            TotalCount = total
        };
    }

    private static IQueryable<Submission> ApplyAccess(IQueryable<Submission> query, Guid userId, string role) => role switch
    {
        // Applying ownership before projection prevents unauthorized rows from leaving PostgreSQL.
        AppRoles.Student => query.Where(x => x.StudentId == userId),
        AppRoles.Teacher => query.Where(x => x.Assignment.CourseOffering.Teachers.Any(t => t.TeacherId == userId)),
        AppRoles.Admin => query,
        _ => query.Where(_ => false)
    };

    private static System.Linq.Expressions.Expression<Func<Submission, SubmissionDto>> Project() => x => new SubmissionDto
    {
        SubmissionId = x.Id,
        AssignmentId = x.AssignmentId,
        AssignmentTitle = x.Assignment.Title,
        StudentUserId = x.StudentId,
        StudentName = x.Student.FullName,
        StudentEmail = x.Student.Email!,
        Answer = x.Answer,
        SubmittedAtUtc = x.SubmittedAtUtc,
        Status = x.Status,
        Marks = x.Marks,
        Feedback = x.Feedback,
        GradedAtUtc = x.GradedAtUtc,
        MaximumMarks = x.Assignment.MaximumMarks
    };
}
