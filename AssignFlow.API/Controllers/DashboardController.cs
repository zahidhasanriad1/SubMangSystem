using AssignFlow.Models.Common;
using AssignFlow.Models.Dashboard;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[Authorize(Roles = AppRoles.All)]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        DashboardSummaryDto data = await _dashboardService.GetSummaryAsync(CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<DashboardSummaryDto>(data));
    }
}
