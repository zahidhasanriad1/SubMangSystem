using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Domain.Enums;
using AssignFlow.Models.Common;
using AssignFlow.Models.Submissions;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Services.Services;

public class SubmissionService : Service<Submission, Guid>, ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseOfferingRepository _courseOfferingRepository;
    private readonly TimeProvider _timeProvider;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        ICourseOfferingRepository courseOfferingRepository,
        TimeProvider timeProvider) : base(submissionRepository)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _courseOfferingRepository = courseOfferingRepository;
        _timeProvider = timeProvider;
    }

    public Task<PagedResultDto<SubmissionDto>> GetPagedAsync(SubmissionFilterDto filter,Guid userId, string role,CancellationToken cancellationToken)
    {
        return _submissionRepository.GetPagedAsync(filter, userId, role, cancellationToken);
    }

    public async Task<SubmissionDto> GetByIdAsync( Guid submissionId,Guid userId,string role,CancellationToken cancellationToken)
    {
        return await _submissionRepository.GetDetailsAsync(submissionId, userId, role, cancellationToken)?? throw new NotFoundException("Submission was not found.");
    }

    public async Task<SubmissionDto> SubmitAsync(Guid assignmentId,UpsertSubmissionDto model, Guid studentId,CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment was not found.");
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (assignment.Status != AssignmentStatus.Published)
            throw new BadRequestException("Only published assignments accept submissions.");

        if (assignment.DeadlineUtc <= now)
            throw new BadRequestException("The submission deadline has passed.");

        if (!await _courseOfferingRepository.IsStudentEnrolledAsync(assignment.CourseOfferingId, studentId, cancellationToken))
            throw new ForbiddenException("You are not enrolled in this course.");

        var submission = await _submissionRepository.GetByAssignmentAndStudentAsync(assignmentId, studentId, cancellationToken);
        var isNew = submission is null;

        if (submission is null)
        {
            submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId
            };
        }
        else
        {
            if (!assignment.AllowResubmission)
                throw new ConflictException("This assignment does not allow resubmission.");
            if (submission.Status == SubmissionStatus.Graded)
                throw new ConflictException("A graded submission cannot be changed.");
        }

        submission.Answer = model.Answer.Trim();
        submission.SubmittedAtUtc = now;
        submission.Status = SubmissionStatus.Submitted;
        submission.Marks = null;
        submission.Feedback = null;
        submission.GradedAtUtc = null;
        submission.GradedById = null;

        var saved = isNew
            ? await _submissionRepository.AddAsync(submission, cancellationToken)
            : await _submissionRepository.UpdateAsync(submission, cancellationToken);
        if (!saved) throw new BadRequestException("Failed to save the submission.");
        return await GetByIdAsync(submission.Id, studentId, AppRoles.Student, cancellationToken);
    }

    public async Task<SubmissionDto> GradeAsync(Guid submissionId,GradeSubmissionDto model,Guid graderId,string role,CancellationToken cancellationToken)
    {
        var submission = await GetManageableAsync(submissionId, graderId, role, cancellationToken);
        if (model.Marks < 0 || model.Marks > submission.Assignment.MaximumMarks)
            throw new BadRequestException($"Marks must be between 0 and {submission.Assignment.MaximumMarks}.");

        submission.Marks = model.Marks;
        submission.Feedback = model.Feedback.Trim();
        submission.Status = SubmissionStatus.Graded;
        submission.GradedById = graderId;
        submission.GradedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;

        _ = await _submissionRepository.UpdateAsync(submission, cancellationToken)? true
            : throw new BadRequestException("Failed to grade the submission.");
        return await GetByIdAsync(submissionId, graderId, role, cancellationToken);
    }

    public async Task<SubmissionDto> ChangeStatusAsync(Guid submissionId,ChangeSubmissionStatusDto model, Guid userId, string role,CancellationToken cancellationToken)
    {
        if (model.Status == SubmissionStatus.Graded)
            throw new BadRequestException("Use the grading endpoint to mark a submission as graded.");
        var submission = await GetManageableAsync(submissionId, userId, role, cancellationToken);
        submission.Status = model.Status;

 _ = await _submissionRepository.UpdateAsync(submission, cancellationToken)? true
            : throw new BadRequestException("Failed to update the submission status.");
        return await GetByIdAsync(submissionId, userId, role, cancellationToken);
    }

    private async Task<Submission> GetManageableAsync(
        Guid submissionId,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        if (role != AppRoles.Teacher) throw new ForbiddenException();
        var submission = await _submissionRepository.GetByIdAsync(submissionId, cancellationToken)
            ?? throw new NotFoundException("Submission was not found.");

        if (!await _courseOfferingRepository.IsTeacherAssignedAsync(
                submission.Assignment.CourseOfferingId, userId, cancellationToken))
            throw new ForbiddenException("Only an assigned teacher can review this submission.");
        return submission;
    }
}
