namespace AssignFlow.Domain.Entities;

public class ClassRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<CourseOffering> CourseOfferings { get; set; } = [];
}
