namespace AssignFlow.Models.Users;

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Role { get; set; } = string.Empty;
}
