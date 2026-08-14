using Microsoft.Extensions.DependencyInjection;

namespace AssignFlow.Services.ServiceInjection;

public interface IServiceInjection
{
    void ServiceInject(IServiceCollection services);
}
