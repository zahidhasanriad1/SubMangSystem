using AssignFlow.Domain.Enums;

namespace AssignFlow.Models.Submissions;

public class SubmissionDto
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public Guid StudentUserId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; set; }
    public SubmissionStatus Status { get; set; }
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAtUtc { get; set; }
    public decimal MaximumMarks { get; set; }
}
