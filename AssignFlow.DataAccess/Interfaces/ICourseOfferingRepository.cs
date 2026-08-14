using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;

namespace AssignFlow.DataAccess.Interfaces;

public interface ICourseOfferingRepository : IRepository<CourseOffering, Guid>
{
    Task<ICollection<CourseOfferingDto>> GetCourseOfferingsAsync(Guid? userId, string? role, CancellationToken cancellationToken = default);
    Task<CourseOfferingDto?> GetCourseOfferingDetailsAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid classRoomId, Guid subjectId, Guid? excludedId, CancellationToken cancellationToken = default);
    Task<bool> IsTeacherAssignedAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentEnrolledAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> AssignTeacherAsync(CourseTeacher entity, CancellationToken cancellationToken = default);
    Task<bool> EnrollStudentAsync(CourseEnrollment entity, CancellationToken cancellationToken = default);
    Task<bool> RemoveTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<bool> RemoveStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default);
}
