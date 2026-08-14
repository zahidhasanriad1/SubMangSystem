using AssignFlow.DataAccess.RepositoryInjection;
using AssignFlow.Services.Interfaces;
using AssignFlow.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AssignFlow.Services.ServiceInjection;

public class ServiceInjection : IServiceInjection
{
    private static readonly IRepositoryInjection RepositoryConfiguration = new RepositoryInjection();

    public void ServiceInject(IServiceCollection services)
    {
        RepositoryConfiguration.RepositoryInject(services);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IClassRoomService, ClassRoomService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ICourseOfferingService, CourseOfferingService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<ISystemSettingService, SystemSettingService>();
        services.AddScoped<IDashboardService, DashboardService>();
    }
}
