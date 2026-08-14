using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.Services.Interfaces;

public interface ICourseOfferingService : IService<CourseOffering, Guid>
{
    Task<ICollection<CourseOfferingDto>> GetCourseOfferingsAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    Task<CourseOfferingDto> CreateCourseOfferingAsync(UpsertCourseOfferingDto model, CancellationToken cancellationToken = default);
    Task<CourseOfferingDto> UpdateCourseOfferingAsync(Guid courseOfferingId, UpsertCourseOfferingDto model, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<bool> AssignTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<bool> EnrollStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> RemoveTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<bool> RemoveStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default);
}
