using AssignFlow.Domain.Entities;
using AssignFlow.Models.Assignments;
using AssignFlow.Models.Common;

namespace AssignFlow.DataAccess.Interfaces;

public interface IAssignmentRepository : IRepository<AssignmentItem, Guid>
{
    Task<AssignmentDto?> GetDetailsAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken);
    Task<PagedResultDto<AssignmentDto>> GetPagedAsync(AssignmentFilterDto filter, Guid userId, string role, CancellationToken cancellationToken);
    Task<bool> HasSubmissionsAsync(Guid assignmentId, CancellationToken cancellationToken);
}
