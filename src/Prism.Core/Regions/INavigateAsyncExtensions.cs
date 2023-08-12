using System;

namespace Prism.Regions
{
    /// <summary>
    /// Provides compatibility extensions for <see cref="INavigateAsync"/>
    /// </summary>
    public static class INavigateAsyncExtensions
    {

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigate">The instance of <see cref="INavigateAsync"/>.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public static void RequestNavigate(this INavigateAsync navigate, Uri target, Action<NavigationResult> navigationCallback) =>
            navigate.RequestNavigate(target, new RegionNavigationCallback(navigationCallback));

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigate">The instance of <see cref="INavigateAsync"/>.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public static void RequestNavigate(this INavigateAsync navigate, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters) =>
            navigate.RequestNavigate(target, new RegionNavigationCallback(navigationCallback), navigationParameters);
    }
}
