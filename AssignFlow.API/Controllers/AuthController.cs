using AssignFlow.Models.Auth;
using AssignFlow.Models.Common;
using AssignFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginRequestDto model,
        CancellationToken cancellationToken)
    {
        AuthResponseDto data = await _authService.LoginAsync(model, cancellationToken);

        return Ok(new ApiResponse<AuthResponseDto>(data));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<CurrentUserDto>>> GetCurrentUser(CancellationToken cancellationToken)
    {
        CurrentUserDto data = await _authService.GetCurrentUserAsync(CurrentUserId, cancellationToken);

        return Ok(new ApiResponse<CurrentUserDto>(data));
    }
}
