using AssignFlow.Models.Common;
using AssignFlow.Models.Submissions;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[Authorize(Roles = AppRoles.All)]
public class SubmissionsController : BaseController
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<SubmissionDto>>>> GetAll([FromQuery] SubmissionFilterDto model, CancellationToken cancellationToken)
    {
        PagedResultDto<SubmissionDto> data = await _submissionService.GetPagedAsync(model, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<PagedResultDto<SubmissionDto>>(data));
    }

    [HttpGet("{submissionId:guid}")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> GetById(Guid submissionId, CancellationToken cancellationToken)
    {
        SubmissionDto data = await _submissionService.GetByIdAsync(submissionId, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<SubmissionDto>(data));
    }

    [Authorize(Roles = AppRoles.Student)]
    [HttpPut("assignment/{assignmentId:guid}")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> Submit(Guid assignmentId,[FromBody] UpsertSubmissionDto model, CancellationToken cancellationToken)
    {
        SubmissionDto data = await _submissionService.SubmitAsync( assignmentId, model, CurrentUserId,cancellationToken);

        return Ok(new ApiResponse<SubmissionDto>(data, message: "Submission saved successfully."));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpPut("{submissionId:guid}/grade")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> Grade( Guid submissionId,[FromBody] GradeSubmissionDto model,CancellationToken cancellationToken)
    {
        SubmissionDto data = await _submissionService.GradeAsync( submissionId, model,CurrentUserId,CurrentRole,cancellationToken);

        return Ok(new ApiResponse<SubmissionDto>(data, message: "Submission graded successfully."));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpPatch("{submissionId:guid}/status")]
    public async Task<ActionResult<ApiResponse<SubmissionDto>>> ChangeStatus( Guid submissionId,[FromBody] ChangeSubmissionStatusDto model,CancellationToken cancellationToken)
    {
        SubmissionDto data = await _submissionService.ChangeStatusAsync(submissionId,model,CurrentUserId,CurrentRole,cancellationToken);

        return Ok(new ApiResponse<SubmissionDto>(data, message: "Submission status updated successfully."));
    }
}
