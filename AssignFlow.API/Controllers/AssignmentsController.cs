using AssignFlow.Models.Assignments;
using AssignFlow.Models.Common;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[Authorize(Roles = AppRoles.All)]
public class AssignmentsController : BaseController
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<AssignmentDto>>>> GetAll(
        [FromQuery] AssignmentFilterDto model,
        CancellationToken cancellationToken)
    {
        PagedResultDto<AssignmentDto> data = await _assignmentService.GetPagedAsync(model, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<PagedResultDto<AssignmentDto>>(data));
    }

    [HttpGet("{assignmentId:guid}")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> GetById(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        AssignmentDto data = await _assignmentService.GetByIdAsync(assignmentId, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<AssignmentDto>(data));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Create(
        [FromBody] CreateAssignmentDto model,
        CancellationToken cancellationToken)
    {
        AssignmentDto data = await _assignmentService.CreateAsync(model, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<AssignmentDto>(data, message: "Assignment created successfully."));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpPut("{assignmentId:guid}")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Update(
        Guid assignmentId,
        [FromBody] UpdateAssignmentDto model,
        CancellationToken cancellationToken)
    {
        AssignmentDto data = await _assignmentService.UpdateAsync(assignmentId, model, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<AssignmentDto>(data, message: "Assignment updated successfully."));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpPatch("{assignmentId:guid}/status")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> ChangeStatus(Guid assignmentId, [FromBody] ChangeAssignmentStatusDto model, CancellationToken cancellationToken)
    {
        AssignmentDto data = await _assignmentService.ChangeStatusAsync(assignmentId, model, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<AssignmentDto>(data, message: "Assignment status updated successfully."));
    }

    [Authorize(Roles = AppRoles.Teacher)]
    [HttpDelete("{assignmentId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        bool result = await _assignmentService.DeleteAsync(assignmentId, CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<bool>(result));
    }
}
