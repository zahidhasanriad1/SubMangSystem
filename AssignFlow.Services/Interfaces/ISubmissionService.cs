using AssignFlow.Domain.Entities;
using AssignFlow.Models.Common;
using AssignFlow.Models.Submissions;

namespace AssignFlow.Services.Interfaces;

public interface ISubmissionService : IService<Submission, Guid>
{
    Task<PagedResultDto<SubmissionDto>> GetPagedAsync(SubmissionFilterDto filter, Guid userId, string role, CancellationToken cancellationToken);
    Task<SubmissionDto> GetByIdAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken);
    Task<SubmissionDto> SubmitAsync(Guid assignmentId, UpsertSubmissionDto request, Guid studentId, CancellationToken cancellationToken);
    Task<SubmissionDto> GradeAsync(Guid id, GradeSubmissionDto request, Guid graderId, string role, CancellationToken cancellationToken);
    Task<SubmissionDto> ChangeStatusAsync(Guid id, ChangeSubmissionStatusDto request, Guid userId, string role, CancellationToken cancellationToken);
}
