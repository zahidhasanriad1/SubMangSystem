namespace AssignFlow.Domain.Entities;

public class CourseTeacher
{
    public Guid CourseOfferingId { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;
    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; } = null!;
    public DateTime AssignedAtUtc { get; set; }
}
