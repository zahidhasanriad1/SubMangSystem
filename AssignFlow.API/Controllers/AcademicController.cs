using AssignFlow.Models.Academic;
using AssignFlow.Models.Common;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[Authorize(Roles = AppRoles.All)]
public class AcademicController : BaseController
{
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly ICourseOfferingService _courseOfferingService;

    public AcademicController(
        IClassRoomService classRoomService,
        ISubjectService subjectService,
        ICourseOfferingService courseOfferingService)
    {
        _classRoomService = classRoomService;
        _subjectService = subjectService;
        _courseOfferingService = courseOfferingService;
    }

    [HttpGet("classes")]
    public async Task<ActionResult<ApiResponse<ICollection<ClassRoomDto>>>> GetClasses(CancellationToken cancellationToken)
    {
        ICollection<ClassRoomDto> data = await _classRoomService.GetClassRoomsAsync(cancellationToken);

        return Ok(new ApiResponse<ICollection<ClassRoomDto>>(data));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("classes")]
    public async Task<ActionResult<ApiResponse<ClassRoomDto>>> CreateClass([FromBody] UpsertClassRoomDto model, CancellationToken cancellationToken)
    {
        ClassRoomDto data = await _classRoomService.CreateClassRoomAsync(model, cancellationToken);

        return Ok(new ApiResponse<ClassRoomDto>(data, message: "Class created successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("classes/{classRoomId:guid}")]
    public async Task<ActionResult<ApiResponse<ClassRoomDto>>> UpdateClass(Guid classRoomId, [FromBody] UpsertClassRoomDto model, CancellationToken cancellationToken)
    {
        ClassRoomDto data = await _classRoomService.UpdateClassRoomAsync(classRoomId, model, cancellationToken);

        return Ok(new ApiResponse<ClassRoomDto>(data, message: "Class updated successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("classes/{classRoomId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClass(Guid classRoomId, CancellationToken cancellationToken)
    {
        bool result = await _classRoomService.DeleteClassRoomAsync(classRoomId, cancellationToken);

        return Ok(new ApiResponse<bool>(result, message: "Class deleted successfully."));
    }

    [HttpGet("subjects")]
    public async Task<ActionResult<ApiResponse<ICollection<SubjectDto>>>> GetSubjects(CancellationToken cancellationToken)
    {
        ICollection<SubjectDto> data = await _subjectService.GetSubjectsAsync(cancellationToken);

        return Ok(new ApiResponse<ICollection<SubjectDto>>(data));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("subjects")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> CreateSubject([FromBody] UpsertSubjectDto model, CancellationToken cancellationToken)
    {
        SubjectDto data = await _subjectService.CreateSubjectAsync(model, cancellationToken);

        return Ok(new ApiResponse<SubjectDto>(data, message: "Subject created successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("subjects/{subjectId:guid}")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> UpdateSubject(Guid subjectId, [FromBody] UpsertSubjectDto model, CancellationToken cancellationToken)
    {
        SubjectDto data = await _subjectService.UpdateSubjectAsync(subjectId, model, cancellationToken);

        return Ok(new ApiResponse<SubjectDto>(data, message: "Subject updated successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("subjects/{subjectId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSubject(Guid subjectId, CancellationToken cancellationToken)
    {
        bool result = await _subjectService.DeleteSubjectAsync(subjectId, cancellationToken);

        return Ok(new ApiResponse<bool>(result, message: "Subject deleted successfully."));
    }

    [HttpGet("course-offerings")]
    public async Task<ActionResult<ApiResponse<ICollection<CourseOfferingDto>>>> GetCourseOfferings(CancellationToken cancellationToken)
    {
        ICollection<CourseOfferingDto> data = await _courseOfferingService.GetCourseOfferingsAsync(CurrentUserId, CurrentRole, cancellationToken);

        return Ok(new ApiResponse<ICollection<CourseOfferingDto>>(data));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("course-offerings")]
    public async Task<ActionResult<ApiResponse<CourseOfferingDto>>> CreateCourseOffering([FromBody] UpsertCourseOfferingDto model, CancellationToken cancellationToken)
    {
        CourseOfferingDto data = await _courseOfferingService.CreateCourseOfferingAsync(model, cancellationToken);

        return Ok(new ApiResponse<CourseOfferingDto>(data, message: "Course offering created successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("course-offerings/{courseOfferingId:guid}")]
    public async Task<ActionResult<ApiResponse<CourseOfferingDto>>> UpdateCourseOffering(Guid courseOfferingId,[FromBody] UpsertCourseOfferingDto model,CancellationToken cancellationToken)
    {
        CourseOfferingDto data = await _courseOfferingService.UpdateCourseOfferingAsync(courseOfferingId, model, cancellationToken);

        return Ok(new ApiResponse<CourseOfferingDto>(data, message: "Course offering updated successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("course-offerings/{courseOfferingId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourseOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        bool result = await _courseOfferingService.DeleteCourseOfferingAsync(courseOfferingId, cancellationToken);

        return Ok(new ApiResponse<bool>(result, message: "Course offering deleted successfully."));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("course-offerings/{courseOfferingId:guid}/teachers/{teacherId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignTeacher(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken)
    {
        bool result = await _courseOfferingService.AssignTeacherAsync(courseOfferingId, teacherId, cancellationToken);

        return Ok(new ApiResponse<bool>(result));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("course-offerings/{courseOfferingId:guid}/teachers/{teacherId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveTeacher(Guid courseOfferingId, Guid teacherId, CancellationToken cancellationToken)
    {
        bool result = await _courseOfferingService.RemoveTeacherAsync(courseOfferingId, teacherId, cancellationToken);

        return Ok(new ApiResponse<bool>(result));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("course-offerings/{courseOfferingId:guid}/students/{studentId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> EnrollStudent(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken)
    {
        bool result = await _courseOfferingService.EnrollStudentAsync(courseOfferingId, studentId, cancellationToken);

        return Ok(new ApiResponse<bool>(result));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("course-offerings/{courseOfferingId:guid}/students/{studentId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveStudent(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken)
    {
        bool result = await _courseOfferingService.RemoveStudentAsync(courseOfferingId, studentId, cancellationToken);

        return Ok(new ApiResponse<bool>(result));
    }
}
