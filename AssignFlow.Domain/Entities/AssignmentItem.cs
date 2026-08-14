using AssignFlow.Domain.Enums;

namespace AssignFlow.Domain.Entities;

public class AssignmentItem
{
    public Guid Id { get; set; }
    public Guid CourseOfferingId { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;
    public Guid CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DeadlineUtc { get; set; }
    public decimal MaximumMarks { get; set; }
    public bool AllowResubmission { get; set; } = true;
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public DateTime? PublishedAtUtc { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Submission> Submissions { get; set; } = [];
}
