using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Academic;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AssignFlow.Services.Services;

public class CourseOfferingService : Service<CourseOffering, Guid>, ICourseOfferingService
{
    private readonly ICourseOfferingRepository _courseOfferingRepository;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TimeProvider _timeProvider;

    public CourseOfferingService(
        ICourseOfferingRepository courseOfferingRepository,
        IClassRoomService classRoomService,
        ISubjectService subjectService,
        UserManager<ApplicationUser> userManager,
        TimeProvider timeProvider) : base(courseOfferingRepository)
    {
        _courseOfferingRepository = courseOfferingRepository;
        _classRoomService = classRoomService;
        _subjectService = subjectService;
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public Task<ICollection<CourseOfferingDto>> GetCourseOfferingsAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return _courseOfferingRepository.GetCourseOfferingsAsync(userId, role, cancellationToken);
    }

    public async Task<CourseOfferingDto> CreateCourseOfferingAsync(
        UpsertCourseOfferingDto model,
        CancellationToken cancellationToken = default)
    {
        _ = await _classRoomService.GetByIdAsync(model.ClassRoomId, cancellationToken);
        _ = await _subjectService.GetByIdAsync(model.SubjectId, cancellationToken);
        if (await _courseOfferingRepository.ExistsAsync(model.ClassRoomId, model.SubjectId, null, cancellationToken))
            throw new ConflictException("The subject is already offered to this class.");

        var entity = new CourseOffering
        {
            ClassRoomId = model.ClassRoomId,
            SubjectId = model.SubjectId,
            IsActive = model.IsActive
        };
        _ = await _courseOfferingRepository.AddAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to create the course offering.");
        return await _courseOfferingRepository.GetCourseOfferingDetailsAsync(entity.Id, cancellationToken)
            ?? throw new NotFoundException("The created course offering could not be loaded.");
    }

    public async Task<CourseOfferingDto> UpdateCourseOfferingAsync(
        Guid courseOfferingId,
        UpsertCourseOfferingDto model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _courseOfferingRepository.GetByIdAsync(courseOfferingId, cancellationToken)
            ?? throw new NotFoundException("Course offering was not found.");
        _ = await _classRoomService.GetByIdAsync(model.ClassRoomId, cancellationToken);
        _ = await _subjectService.GetByIdAsync(model.SubjectId, cancellationToken);

        if (await _courseOfferingRepository.ExistsAsync(model.ClassRoomId, model.SubjectId, courseOfferingId, cancellationToken))
            throw new ConflictException("The subject is already offered to this class.");

        entity.ClassRoomId = model.ClassRoomId;
        entity.SubjectId = model.SubjectId;
        entity.IsActive = model.IsActive;
        entity.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;
        _ = await _courseOfferingRepository.UpdateAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to update the course offering.");

        return await _courseOfferingRepository.GetCourseOfferingDetailsAsync(courseOfferingId, cancellationToken)
            ?? throw new NotFoundException("The updated course offering could not be loaded.");
    }

    public async Task<bool> DeleteCourseOfferingAsync(
        Guid courseOfferingId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _courseOfferingRepository.GetByIdAsync(courseOfferingId, cancellationToken)
            ?? throw new NotFoundException("Course offering was not found.");
        if (await _courseOfferingRepository.HasAssignmentsAsync(courseOfferingId, cancellationToken))
            throw new ConflictException("Delete the course's assignments before deleting the course offering.");

        return await _courseOfferingRepository.DeleteAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to delete the course offering.");
    }

    public async Task<bool> AssignTeacherAsync(
        Guid courseOfferingId,
        Guid teacherId,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserRoleAsync(teacherId, AppRoles.Teacher);
        _ = await GetByIdAsync(courseOfferingId, cancellationToken);
        if (await _courseOfferingRepository.IsTeacherAssignedAsync(courseOfferingId, teacherId, cancellationToken))
            return true;

        return await _courseOfferingRepository.AssignTeacherAsync(new CourseTeacher
        {
            CourseOfferingId = courseOfferingId,
            TeacherId = teacherId,
            AssignedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        }, cancellationToken);
    }

    public async Task<bool> EnrollStudentAsync(
        Guid courseOfferingId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserRoleAsync(studentId, AppRoles.Student);
        _ = await GetByIdAsync(courseOfferingId, cancellationToken);
        if (await _courseOfferingRepository.IsStudentEnrolledAsync(courseOfferingId, studentId, cancellationToken))
            return true;

        return await _courseOfferingRepository.EnrollStudentAsync(new CourseEnrollment
        {
            CourseOfferingId = courseOfferingId,
            StudentId = studentId,
            EnrolledAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        }, cancellationToken);
    }

    public Task<bool> RemoveTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        return _courseOfferingRepository.RemoveTeacherAsync(courseOfferingId, teacherId, cancellationToken);
    }

    public Task<bool> RemoveStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return _courseOfferingRepository.RemoveStudentAsync(courseOfferingId, studentId, cancellationToken);
    }

    private async Task EnsureUserRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User was not found.");
        if (!user.IsActive || !await _userManager.IsInRoleAsync(user, role))
            throw new BadRequestException($"The selected user is not an active {role}.");
    }
}
