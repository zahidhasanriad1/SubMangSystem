namespace AssignFlow.Models.Settings;

public class SystemSettingDto
{
    public Guid SystemSettingId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
}
