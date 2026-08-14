using AssignFlow.Domain.Enums;

namespace AssignFlow.Models.Assignments;

public class CreateAssignmentDto
{
    public Guid CourseOfferingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DeadlineUtc { get; set; }
    public decimal MaximumMarks { get; set; }
    public bool AllowResubmission { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
}
