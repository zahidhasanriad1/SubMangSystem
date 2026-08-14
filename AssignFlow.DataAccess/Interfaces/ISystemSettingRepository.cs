using AssignFlow.Domain.Entities;
using AssignFlow.Models.Settings;

namespace AssignFlow.DataAccess.Interfaces;

public interface ISystemSettingRepository : IRepository<SystemSetting, Guid>
{
    Task<ICollection<SystemSettingDto>> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
}
