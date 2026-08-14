namespace AssignFlow.Models.Academic;

public class UpsertCourseOfferingDto
{
    public Guid ClassRoomId { get; set; }
    public Guid SubjectId { get; set; }
    public bool IsActive { get; set; } = true;
}
