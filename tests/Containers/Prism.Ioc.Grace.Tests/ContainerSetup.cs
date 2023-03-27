using System;
using Grace.DependencyInjection;
using Prism.Grace;

namespace Prism.Ioc.Tests
{
    partial class ContainerSetup
    {
        protected virtual IContainerExtension CreateContainerInternal() => new GraceContainerExtension();

        public Type NativeContainerType => typeof(DependencyInjectionContainer);
    }
}
