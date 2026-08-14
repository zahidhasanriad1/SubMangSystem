using AssignFlow.Models.Common;
using AssignFlow.Models.Users;

namespace AssignFlow.Services.Interfaces;

public interface IAdminService
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(PagedRequestDto request, CancellationToken cancellationToken);
    Task<UserDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto request, CancellationToken cancellationToken);
}
