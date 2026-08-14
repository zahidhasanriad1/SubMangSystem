using AssignFlow.Models.Common;
using AssignFlow.Models.Users;

namespace AssignFlow.DataAccess.Interfaces;

public interface IAdminRepository
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(PagedRequestDto request, CancellationToken cancellationToken);
}
