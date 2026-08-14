using AssignFlow.Models.Common;
using AssignFlow.Models.Settings;
using AssignFlow.Models.Users;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : BaseController
{
    private readonly IAdminService _adminService;
    private readonly ISystemSettingService _systemSettingService;

    public AdminController(IAdminService adminService, ISystemSettingService systemSettingService)
    {
        _adminService = adminService;
        _systemSettingService = systemSettingService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<UserDto>>>> GetUsers([FromQuery] PagedRequestDto model,CancellationToken cancellationToken)
    {
        PagedResultDto<UserDto> data = await _adminService.GetUsersAsync(model, cancellationToken);

        return Ok(new ApiResponse<PagedResultDto<UserDto>>(data));
    }

    [HttpPost("users")]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto model, CancellationToken cancellationToken)
    {
        UserDto data = await _adminService.CreateUserAsync(model, cancellationToken);

        return Ok(new ApiResponse<UserDto>(data, message: "User created successfully."));
    }

    [HttpPut("users/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(Guid userId,[FromBody] UpdateUserDto model, CancellationToken cancellationToken)
    {
        UserDto data = await _adminService.UpdateUserAsync(userId, model, cancellationToken);

        return Ok(new ApiResponse<UserDto>(data, message: "User updated successfully."));
    }

    [HttpGet("settings")]
    public async Task<ActionResult<ApiResponse<ICollection<SystemSettingDto>>>> GetSettings(CancellationToken cancellationToken)
    {
        ICollection<SystemSettingDto> data = await _systemSettingService.GetSettingsAsync(cancellationToken);

        return Ok(new ApiResponse<ICollection<SystemSettingDto>>(data));
    }

    [HttpPut("settings/{key}")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> UpsertSetting(string key,[FromBody] UpsertSettingDto model,CancellationToken cancellationToken)
    {
        SystemSettingDto data = await _systemSettingService.UpsertSettingAsync(key, model, cancellationToken);

        return Ok(new ApiResponse<SystemSettingDto>(data, message: "Setting saved successfully."));
    }
}
