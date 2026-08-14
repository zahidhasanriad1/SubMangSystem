namespace AssignFlow.Domain.Entities;

public class CourseEnrollment
{
    public Guid CourseOfferingId { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;
    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
    public DateTime EnrolledAtUtc { get; set; }
}
