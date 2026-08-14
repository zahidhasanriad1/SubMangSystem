using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Assignments;
using AssignFlow.Models.Common;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Services.Services;

public class AssignmentService : Service<AssignmentItem, Guid>, IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseOfferingRepository _courseOfferingRepository;
    private readonly TimeProvider _timeProvider;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ICourseOfferingRepository courseOfferingRepository,
        TimeProvider timeProvider) : base(assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
        _courseOfferingRepository = courseOfferingRepository;
        _timeProvider = timeProvider;
    }

    public Task<PagedResultDto<AssignmentDto>> GetPagedAsync(
        AssignmentFilterDto filter,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        return _assignmentRepository.GetPagedAsync(filter, userId, role, cancellationToken);
    }

    public async Task<AssignmentDto> GetByIdAsync(
        Guid assignmentId,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        return await _assignmentRepository.GetDetailsAsync(assignmentId, userId, role, cancellationToken)
            ?? throw new NotFoundException("Assignment was not found.");
    }

    public async Task<AssignmentDto> CreateAsync(
        CreateAssignmentDto model,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        EnsureTeacherRole(role);
        _ = await _courseOfferingRepository.GetByIdAsync(model.CourseOfferingId, cancellationToken)
            ?? throw new NotFoundException("Course offering was not found.");
        await EnsureCourseAccessAsync(model.CourseOfferingId, userId, cancellationToken);
        ValidateDeadlineAndMarks(model.DeadlineUtc, model.MaximumMarks, model.Status);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var entity = new AssignmentItem
        {
            CourseOfferingId = model.CourseOfferingId,
            CreatedById = userId,
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            DeadlineUtc = model.DeadlineUtc.ToUniversalTime(),
            MaximumMarks = model.MaximumMarks,
            AllowResubmission = model.AllowResubmission,
            Status = model.Status,
            PublishedAtUtc = model.Status == AssignmentStatus.Published ? now : null
        };
        _ = await _assignmentRepository.AddAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to create the assignment.");
        return await GetByIdAsync(entity.Id, userId, role, cancellationToken);
    }

    public async Task<AssignmentDto> UpdateAsync(
        Guid assignmentId,
        UpdateAssignmentDto model,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var entity = await GetManageableAsync(assignmentId, userId, role, cancellationToken);
        ValidateDeadlineAndMarks(model.DeadlineUtc, model.MaximumMarks, entity.Status);
        entity.Title = model.Title.Trim();
        entity.Description = model.Description.Trim();
        entity.DeadlineUtc = model.DeadlineUtc.ToUniversalTime();
        entity.MaximumMarks = model.MaximumMarks;
        entity.AllowResubmission = model.AllowResubmission;
        _ = await _assignmentRepository.UpdateAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to update the assignment.");
        return await GetByIdAsync(assignmentId, userId, role, cancellationToken);
    }

    public async Task<AssignmentDto> ChangeStatusAsync(
        Guid assignmentId,
        ChangeAssignmentStatusDto model,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var entity = await GetManageableAsync(assignmentId, userId, role, cancellationToken);
        if (model.Status == AssignmentStatus.Published && entity.DeadlineUtc <= _timeProvider.GetUtcNow().UtcDateTime)
            throw new BadRequestException("An assignment cannot be published after its deadline.");
        if (entity.Status == AssignmentStatus.Archived && model.Status != AssignmentStatus.Archived)
            throw new BadRequestException("Archived assignments cannot be reopened.");
        if (model.Status == AssignmentStatus.Draft && await _assignmentRepository.HasSubmissionsAsync(assignmentId, cancellationToken))
            throw new ConflictException("An assignment with submissions cannot be returned to draft status.");

        entity.Status = model.Status;
        if (model.Status == AssignmentStatus.Published)
            entity.PublishedAtUtc ??= _timeProvider.GetUtcNow().UtcDateTime;
        _ = await _assignmentRepository.UpdateAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to update the assignment status.");
        return await GetByIdAsync(assignmentId, userId, role, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid assignmentId, Guid userId, string role, CancellationToken cancellationToken)
    {
        var entity = await GetManageableAsync(assignmentId, userId, role, cancellationToken);
        if (entity.Status != AssignmentStatus.Draft)
            throw new BadRequestException("Only draft assignments can be deleted.");
        if (await _assignmentRepository.HasSubmissionsAsync(assignmentId, cancellationToken))
            throw new ConflictException("Assignments with submissions cannot be deleted.");
        return await _assignmentRepository.DeleteAsync(entity, cancellationToken)
            ? true
            : throw new BadRequestException("Failed to delete the assignment.");
    }

    private async Task<AssignmentItem> GetManageableAsync(
        Guid assignmentId,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        EnsureTeacherRole(role);
        var entity = await _assignmentRepository.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment was not found.");
        await EnsureCourseAccessAsync(entity.CourseOfferingId, userId, cancellationToken);
        return entity;
    }

    private async Task EnsureCourseAccessAsync(Guid courseOfferingId, Guid userId, CancellationToken cancellationToken)
    {
        if (!await _courseOfferingRepository.IsTeacherAssignedAsync(courseOfferingId, userId, cancellationToken))
            throw new ForbiddenException("Only an assigned teacher can manage this course's assignments.");
    }

    private static void EnsureTeacherRole(string role)
    {
        if (role != AppRoles.Teacher) throw new ForbiddenException();
    }

    private void ValidateDeadlineAndMarks(DateTime deadline, decimal maximumMarks, AssignmentStatus status)
    {
        if (maximumMarks <= 0 || maximumMarks > 10000)
            throw new BadRequestException("Maximum marks must be between 0 and 10,000.");
        if (status == AssignmentStatus.Published && deadline.ToUniversalTime() <= _timeProvider.GetUtcNow().UtcDateTime)
            throw new BadRequestException("Published assignments require a future deadline.");
    }
}
