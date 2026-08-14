namespace AssignFlow.Domain.Entities;

public class CourseOffering
{
    public Guid Id { get; set; }
    public Guid ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<CourseTeacher> Teachers { get; set; } = [];
    public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
    public ICollection<AssignmentItem> Assignments { get; set; } = [];
}
