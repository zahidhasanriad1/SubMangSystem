namespace AssignFlow.Models.Academic;

public class SubjectDto
{
    public Guid SubjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
