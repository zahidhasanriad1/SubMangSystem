using AssignFlow.Domain.Enums;

namespace AssignFlow.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public AssignmentItem Assignment { get; set; } = null!;
    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
    public string Answer { get; set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public Guid? GradedById { get; set; }
    public ApplicationUser? GradedBy { get; set; }
    public DateTime? GradedAtUtc { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
