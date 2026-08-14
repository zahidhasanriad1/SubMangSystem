using Microsoft.AspNetCore.Identity;

namespace AssignFlow.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<CourseTeacher> TeachingAssignments { get; set; } = [];
    public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
}
