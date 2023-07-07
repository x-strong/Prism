using Prism.Common;
using Prism.Ioc;

namespace Prism.Regions
{
    public class RegionViewRegistry : RegionViewRegistryBase
    {
        public RegionViewRegistry(IContainerExtension container)
            : base(container)
        {
        }

        protected override void AutowireViewModel(object view) =>
            MvvmHelpers.AutowireViewModel(view);
    }
}
