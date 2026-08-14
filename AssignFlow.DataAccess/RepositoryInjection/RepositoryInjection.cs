using AssignFlow.DataAccess.Interfaces;
using AssignFlow.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AssignFlow.DataAccess.RepositoryInjection;

public class RepositoryInjection : IRepositoryInjection
{
    public void RepositoryInject(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ICourseOfferingRepository, CourseOfferingRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
    }
}
