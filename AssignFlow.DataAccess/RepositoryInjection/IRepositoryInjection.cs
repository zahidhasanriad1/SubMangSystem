using Microsoft.Extensions.DependencyInjection;

namespace AssignFlow.DataAccess.RepositoryInjection;

public interface IRepositoryInjection
{
    void RepositoryInject(IServiceCollection services);
}
