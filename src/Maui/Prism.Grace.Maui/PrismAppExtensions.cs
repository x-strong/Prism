using Prism;
using Prism.Grace;

namespace Microsoft.Maui;

/// <summary>
/// Application base class using Grace Ioc
/// </summary>
public static class PrismAppExtensions
{
    /// <summary>
    /// Creates a new Prism Application using the Grace Ioc Container.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configurePrism"></param>
    /// <returns></returns>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Action<PrismAppBuilder> configurePrism)
    {
        return builder.UsePrism(new GraceContainerExtension(), configurePrism);
    }
}
