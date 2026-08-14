namespace AssignFlow.Utils.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string AdminOrTeacher = Admin + "," + Teacher;
    public const string All = Admin + "," + Teacher + "," + Student;
}
