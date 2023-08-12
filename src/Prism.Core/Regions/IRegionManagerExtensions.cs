using System;

namespace Prism.Regions
{
    /// <summary>
    /// Common Extensions for the RegionManager
    /// </summary>
    public static class IRegionManagerExtensions
    {
        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <typeparam name="T">The type of the view to register with the  <see cref="IRegion"/>.</typeparam>
        /// <param name="regionManager">The current <see cref="IRegionManager"/>.</param>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public static IRegionManager RegisterViewWithRegion<T>(this IRegionManager regionManager, string regionName) =>
            regionManager.RegisterViewWithRegion(regionName, typeof(T));

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="regionManager">The current <see cref="IRegionManager"/>.</param>
        /// <param name="regionName">The name of the Region to navigate to.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback) =>
            regionManager.RequestNavigate(regionName, target, new RegionNavigationCallback(navigationCallback));

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="regionManager">The current <see cref="IRegionManager"/>.</param>
        /// <param name="regionName">The name of the Region to navigate to.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters) =>
            regionManager.RequestNavigate(regionName, target, new RegionNavigationCallback(navigationCallback), navigationParameters);
    }
}
