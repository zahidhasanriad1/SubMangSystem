namespace AssignFlow.Models.Assignments;

public class UpdateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DeadlineUtc { get; set; }
    public decimal MaximumMarks { get; set; }
    public bool AllowResubmission { get; set; }
}
