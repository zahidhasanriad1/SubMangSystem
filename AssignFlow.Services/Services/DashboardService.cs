using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Models.Dashboard;
using AssignFlow.Services.Interfaces;

namespace AssignFlow.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<DashboardSummaryDto> GetSummaryAsync(Guid userId, string role, CancellationToken cancellationToken) =>
        _dashboardRepository.GetSummaryAsync(userId, role, cancellationToken);
}
