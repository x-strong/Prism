using System;
using System.Collections.Generic;
using Prism.Ioc;

namespace Prism.Regions
{
    /// <summary>
    /// Compatibility Extensions for the <see cref="IRegionViewRegistry"/>
    /// </summary>
    public static class IRegionViewRegistryExtensions
    {
        /// <summary>
        /// Returns the contents associated with a region name.
        /// </summary>
        /// <param name="registry">The <see cref="IRegionViewRegistry"/>.</param>
        /// <param name="regionName">Region name for which contents are requested.</param>
        /// <returns>Collection of contents associated with the <paramref name="regionName"/>.</returns>
        public static IEnumerable<object> GetContents(this IRegionViewRegistry registry, string regionName) =>
            registry.GetContents(regionName, ContainerLocator.Container);

        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name. 
        /// </summary>
        /// <param name="registry">The <see cref="IRegionViewRegistry"/>.</param>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public static void RegisterViewWithRegion(this IRegionViewRegistry registry, string regionName, Func<object> getContentDelegate) =>
            registry.RegisterViewWithRegion(regionName, _ => getContentDelegate());
    }
}
