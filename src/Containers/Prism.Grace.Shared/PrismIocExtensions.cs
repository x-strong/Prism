using Grace.DependencyInjection;
using Prism.Ioc;

namespace Prism.Grace
{
    /// <summary>
    /// Extensions help get the underlying <see cref="IContainer" />
    /// </summary>
    public static class PrismIocExtensions
    {
        /// <summary>
        /// Gets the <see cref="IContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerProvider">The current <see cref="IContainerProvider" /></param>
        /// <returns>The underlying <see cref="IContainer" /></returns>
        public static DependencyInjectionContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<DependencyInjectionContainer>)containerProvider).Instance;
        }

        /// <summary>
        /// Gets the <see cref="IContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <returns>The underlying <see cref="IContainer" /></returns>
        public static DependencyInjectionContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<DependencyInjectionContainer>)containerRegistry).Instance;
        }
    }
}
