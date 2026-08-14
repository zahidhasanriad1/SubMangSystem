using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class SystemSettingRepository : Repository<SystemSetting, Guid>, ISystemSettingRepository
{
    public SystemSettingRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<SystemSettingDto>> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.SystemSettings.AsNoTracking()
            .OrderBy(x => x.Key)
            .Select(x => new SystemSettingDto
            {
                SystemSettingId = x.Id,
                Key = x.Key,
                Value = x.Value,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }

    public Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return DbContext.SystemSettings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    }
}
