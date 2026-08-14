using System.Security.Claims;

namespace AssignFlow.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The access token does not contain a valid user identifier.");
    }

    public static string GetRole(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Role)
        ?? throw new UnauthorizedAccessException("The access token does not contain a role.");
}
