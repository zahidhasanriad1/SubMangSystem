using AssignFlow.Models.Dashboard;

namespace AssignFlow.DataAccess.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(Guid userId, string role, CancellationToken cancellationToken);
}
