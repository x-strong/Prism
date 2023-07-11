using Prism.Ioc;

namespace Prism.Grace;

public abstract class PrismBootstrapper : PrismBootstrapperBase
{
    protected override IContainerExtension CreateContainerExtension() =>
        new GraceContainerExtension();
}
