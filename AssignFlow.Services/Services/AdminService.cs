using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Entities;
using AssignFlow.Models.Common;
using AssignFlow.Models.Users;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using AssignFlow.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AssignFlow.Services.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AdminService(
        IAdminRepository adminRepository,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _adminRepository = adminRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public Task<PagedResultDto<UserDto>> GetUsersAsync(PagedRequestDto request, CancellationToken cancellationToken) =>
        _adminRepository.GetUsersAsync(request, cancellationToken);

    public async Task<UserDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken)
    {
        var role = NormalizeRole(request.Role);
        if (!await _roleManager.RoleExistsAsync(role))
            throw new BadRequestException("The selected role does not exist.");
        if (await _userManager.FindByEmailAsync(request.Email.Trim()) is not null)
            throw new ConflictException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            UserName = request.Email.Trim().ToLowerInvariant(),
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(x => x.Description)));
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            // Compensate for a failed role assignment so no unusable account remains in the database.
            await _userManager.DeleteAsync(user);
            throw new BadRequestException(string.Join(" ", roleResult.Errors.Select(x => x.Description)));
        }

        return MapUser(user, role);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto request, CancellationToken cancellationToken)
    {
        var role = NormalizeRole(request.Role);
        var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User was not found.");
        user.FullName = request.FullName.Trim();
        user.IsActive = request.IsActive;
        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new BadRequestException(string.Join(" ", update.Errors.Select(x => x.Description)));

        var existingRoles = await _userManager.GetRolesAsync(user);
        if (!existingRoles.Contains(role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new BadRequestException(string.Join(" ", roleResult.Errors.Select(x => x.Description)));
            if (existingRoles.Count > 0)
                await _userManager.RemoveFromRolesAsync(user, existingRoles);
        }

        return MapUser(user, role);
    }

    private static string NormalizeRole(string role) => role.Trim().ToLowerInvariant() switch
    {
        "admin" => AppRoles.Admin,
        "teacher" => AppRoles.Teacher,
        "student" => AppRoles.Student,
        _ => throw new BadRequestException("Role must be Admin, Teacher, or Student.")
    };

    private static UserDto MapUser(ApplicationUser user, string role)
    {
        return new UserDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = role,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAt
        };
    }
}
