using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Settings;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Exceptions;

namespace AssignFlow.Services.Services;

public class SystemSettingService : Service<SystemSetting, Guid>, ISystemSettingService
{
    private readonly ISystemSettingRepository _systemSettingRepository;

    public SystemSettingService(ISystemSettingRepository systemSettingRepository) : base(systemSettingRepository)
    {
        _systemSettingRepository = systemSettingRepository;
    }

    public Task<ICollection<SystemSettingDto>> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        return _systemSettingRepository.GetSettingsAsync(cancellationToken);
    }

    public async Task<SystemSettingDto> UpsertSettingAsync(
        string key,
        UpsertSettingDto model,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim().ToUpperInvariant();
        var setting = await _systemSettingRepository.GetByKeyAsync(normalizedKey, cancellationToken);

        if (setting is null)
        {
            setting = new SystemSetting
            {
                Key = normalizedKey,
                Value = model.Value.Trim(),
                Description = model.Description?.Trim()
            };
            _ = await _systemSettingRepository.AddAsync(setting, cancellationToken)
                ? true
                : throw new BadRequestException("Failed to create the setting.");
        }
        else
        {
            setting.Value = model.Value.Trim();
            setting.Description = model.Description?.Trim();
            _ = await _systemSettingRepository.UpdateAsync(setting, cancellationToken)
                ? true
                : throw new BadRequestException("Failed to update the setting.");
        }

        return new SystemSettingDto
        {
            SystemSettingId = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description
        };
    }
}
