using Grace.DependencyInjection;

namespace Prism.Ioc.Tests
{
    public static class ContainerHelpers
    {
        public static DependencyInjectionContainer GetContainer(this IContainerExtension container) =>
            Prism.Grace.PrismIocExtensions.GetContainer((IContainerProvider)container);
    }
}
