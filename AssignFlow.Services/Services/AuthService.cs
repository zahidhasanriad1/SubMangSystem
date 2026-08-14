using AssignFlow.Domain.Entities;
using AssignFlow.Models.Auth;
using AssignFlow.Services.Interfaces;
using AssignFlow.Services.Options;
using AssignFlow.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssignFlow.Services.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwt;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _jwt = jwtOptions.Value;
        _timeProvider = timeProvider;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim())
            ?? throw new UnauthorizedException("Email or password is incorrect.");

        // The response remains deliberately generic to prevent account-enumeration attacks.
        if (!user.IsActive || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Email or password is incorrect.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.SingleOrDefault() ?? throw new BadRequestException("The account has no assigned role.");
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_jwt.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims, now, expires, credentials);
        var currentUser = MapCurrentUser(user, role);
        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expires,
            User = currentUser
        };
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User was not found.");
        var role = (await _userManager.GetRolesAsync(user)).SingleOrDefault() ?? string.Empty;
        return MapCurrentUser(user, role);
    }

    private static CurrentUserDto MapCurrentUser(ApplicationUser user, string role)
    {
        return new CurrentUserDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = role
        };
    }
}
