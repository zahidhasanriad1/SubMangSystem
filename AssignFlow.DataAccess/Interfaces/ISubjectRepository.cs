using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.DataAccess.Interfaces;

public interface ISubjectRepository : IRepository<Subject, Guid>
{
    Task<ICollection<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludedId, CancellationToken cancellationToken = default);
    Task<bool> HasCourseOfferingsAsync(Guid subjectId, CancellationToken cancellationToken = default);
}
