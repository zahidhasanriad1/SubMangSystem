using AssignFlow.Domain.Entities;
using AssignFlow.Models.Common;
using AssignFlow.Models.Submissions;

namespace AssignFlow.DataAccess.Interfaces;

public interface ISubmissionRepository : IRepository<Submission, Guid>
{
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken);
    Task<SubmissionDto?> GetDetailsAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken);
    Task<PagedResultDto<SubmissionDto>> GetPagedAsync(SubmissionFilterDto filter, Guid userId, string role, CancellationToken cancellationToken);
}
