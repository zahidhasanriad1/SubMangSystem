namespace AssignFlow.Models.Academic;

public class CourseOfferingDto
{
    public Guid CourseOfferingId { get; set; }
    public Guid ClassRoomId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TeacherCount { get; set; }
    public int StudentCount { get; set; }
}
