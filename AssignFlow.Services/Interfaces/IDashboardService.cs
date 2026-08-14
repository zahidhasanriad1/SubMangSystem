using AssignFlow.Models.Dashboard;

namespace AssignFlow.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(Guid userId, string role, CancellationToken cancellationToken);
}
