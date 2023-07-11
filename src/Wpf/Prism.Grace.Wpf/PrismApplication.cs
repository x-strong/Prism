using Prism.Ioc;

namespace Prism.Grace;

public abstract class PrismApplication : PrismApplicationBase
{
    protected override IContainerExtension CreateContainerExtension() =>
        new GraceContainerExtension();
}
