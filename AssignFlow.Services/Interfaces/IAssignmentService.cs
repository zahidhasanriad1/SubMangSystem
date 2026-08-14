using AssignFlow.Domain.Entities;
using AssignFlow.Models.Assignments;
using AssignFlow.Models.Common;

namespace AssignFlow.Services.Interfaces;

public interface IAssignmentService : IService<AssignmentItem, Guid>
{
    Task<PagedResultDto<AssignmentDto>> GetPagedAsync(AssignmentFilterDto filter, Guid userId, string role, CancellationToken cancellationToken);
    Task<AssignmentDto> GetByIdAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken);
    Task<AssignmentDto> CreateAsync(CreateAssignmentDto request, Guid userId, string role, CancellationToken cancellationToken);
    Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto request, Guid userId, string role, CancellationToken cancellationToken);
    Task<AssignmentDto> ChangeStatusAsync(Guid id, ChangeAssignmentStatusDto request, Guid userId, string role, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, Guid userId, string role, CancellationToken cancellationToken);
}
