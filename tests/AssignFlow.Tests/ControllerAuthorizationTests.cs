using AssignFlow.API.Controllers;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace AssignFlow.Tests;

public class ControllerAuthorizationTests
{
    [Fact]
    public void AdminController_RequiresAdminRole()
    {
        Assert.Equal(AppRoles.Admin, GetControllerRoles<AdminController>());
    }

    [Theory]
    [InlineData(nameof(AcademicController.DeleteClass))]
    [InlineData(nameof(AcademicController.DeleteSubject))]
    [InlineData(nameof(AcademicController.DeleteCourseOffering))]
    public void AcademicDeleteEndpoints_RequireAdminRole(string methodName)
    {
        Assert.Equal(AppRoles.Admin, GetMethodRoles<AcademicController>(methodName));
    }

    [Theory]
    [InlineData(nameof(AssignmentsController.Create))]
    [InlineData(nameof(AssignmentsController.Update))]
    [InlineData(nameof(AssignmentsController.ChangeStatus))]
    [InlineData(nameof(AssignmentsController.Delete))]
    public void AssignmentWriteEndpoints_RequireTeacherRole(string methodName)
    {
        Assert.Equal(AppRoles.Teacher, GetMethodRoles<AssignmentsController>(methodName));
    }

    [Fact]
    public void SubmissionWriteEndpoints_RequireWorkflowRoles()
    {
        Assert.Equal(AppRoles.Student, GetMethodRoles<SubmissionsController>(nameof(SubmissionsController.Submit)));
        Assert.Equal(AppRoles.Teacher, GetMethodRoles<SubmissionsController>(nameof(SubmissionsController.Grade)));
        Assert.Equal(AppRoles.Teacher, GetMethodRoles<SubmissionsController>(nameof(SubmissionsController.ChangeStatus)));
    }

    private static string? GetControllerRoles<TController>()
    {
        return typeof(TController).GetCustomAttribute<AuthorizeAttribute>()?.Roles;
    }

    private static string? GetMethodRoles<TController>(string methodName)
    {
        return typeof(TController).GetMethod(methodName)?.GetCustomAttribute<AuthorizeAttribute>()?.Roles;
    }
}
