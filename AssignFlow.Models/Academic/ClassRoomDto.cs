namespace AssignFlow.Models.Academic;

public class ClassRoomDto
{
    public Guid ClassRoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public bool IsActive { get; set; }
}
