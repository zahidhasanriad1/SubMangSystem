using AssignFlow.Domain.Enums;

namespace AssignFlow.Models.Assignments;

public class AssignmentDto
{
    public Guid AssignmentId { get; set; }
    public Guid CourseOfferingId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DeadlineUtc { get; set; }
    public decimal MaximumMarks { get; set; }
    public bool AllowResubmission { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
    public bool HasSubmitted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
