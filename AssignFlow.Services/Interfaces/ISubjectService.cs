using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.Services.Interfaces;

public interface ISubjectService : IService<Subject, Guid>
{
    Task<ICollection<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<SubjectDto> CreateSubjectAsync(UpsertSubjectDto model, CancellationToken cancellationToken = default);
    Task<SubjectDto> UpdateSubjectAsync(Guid subjectId, UpsertSubjectDto model, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);
}
