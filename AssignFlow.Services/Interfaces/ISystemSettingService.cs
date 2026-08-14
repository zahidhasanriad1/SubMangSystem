using AssignFlow.Domain.Entities;
using AssignFlow.Models.Settings;

namespace AssignFlow.Services.Interfaces;

public interface ISystemSettingService : IService<SystemSetting, Guid>
{
    Task<ICollection<SystemSettingDto>> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingDto> UpsertSettingAsync(string key, UpsertSettingDto model, CancellationToken cancellationToken = default);
}
