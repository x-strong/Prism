using Prism.Ioc;
using Grace.DependencyInjection.Extensions;

namespace Prism.Grace;

public partial class GraceContainerExtension : IServiceCollectionAware
{
    IServiceProvider _serviceProvider;

    public IServiceProvider CreateServiceProvider() =>
        _serviceProvider;

    public void Populate(IServiceCollection services) =>
        _serviceProvider = Instance.Populate(services);
}
