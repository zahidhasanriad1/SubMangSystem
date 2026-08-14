namespace AssignFlow.Models.Dashboard;

public class DashboardSummaryDto
{
    public int Users { get; set; }
    public int Courses { get; set; }
    public int Assignments { get; set; }
    public int PublishedAssignments { get; set; }
    public int Submissions { get; set; }
    public int PendingReviews { get; set; }
}
