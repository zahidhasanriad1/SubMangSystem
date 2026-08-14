using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using AssignFlow.Models.Common;
using AssignFlow.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.DataAccess.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _dbContext;

    public AdminRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersAsync(PagedRequestDto request, CancellationToken cancellationToken)
    {
        var query = from user in _dbContext.Users.AsNoTracking()
                    join userRole in _dbContext.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
                    join role in _dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                    select new { user, Role = role.Name! };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.user.FullName, $"%{term}%") || EF.Functions.ILike(x.user.Email!, $"%{term}%"));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.user.FullName)
            .Skip((request.SafePage - 1) * request.SafePageSize).Take(request.SafePageSize)
            .Select(x => new UserDto
            {
                UserId = x.user.Id,
                FullName = x.user.FullName,
                Email = x.user.Email!,
                Role = x.Role,
                IsActive = x.user.IsActive,
                CreatedAtUtc = x.user.CreatedAt
            })
            .ToListAsync(cancellationToken);
        return new PagedResultDto<UserDto>
        {
            Items = items,
            Page = request.SafePage,
            PageSize = request.SafePageSize,
            TotalCount = total
        };
    }

}
