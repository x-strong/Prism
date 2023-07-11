using System;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Prism.Ioc;
using Prism.Ioc.Internals;

namespace Prism.Grace
{
    /// <summary>
    /// The <see cref="IContainerExtension" /> Implementation to use with DryIoc
    /// </summary>
#if ContainerExtensions
    internal
#else
    public
#endif
    partial class GraceContainerExtension : IContainerExtension<DependencyInjectionContainer>, IContainerInfo
    {
        public GraceContainerExtension()
        {
            Instance = new DependencyInjectionContainer();

            Instance.Configure(c => c.ExportInstance<IContainerProvider>(this));
            Instance.Configure(c => c.ExportInstance<IContainerExtension>(this));

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(LocateException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(NullValueProvidedException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(RecursiveLocateException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ImportInjectionScopeException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(NamedScopeLocateException));
        }

        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public DependencyInjectionContainer Instance { get; }
        public IScopedProvider CurrentScope { get; }

        public IScopedProvider CreateScope()
        {
            var injectionScope = Instance.CreateChildScope();
            return new GraceScopedContainerProvider(injectionScope, this);
        }

        public void FinalizeExtension()
        {
        }

        public bool IsRegistered(Type type) =>
            Instance.CanLocate(type);

        public bool IsRegistered(Type type, string name) =>
            Instance.CanLocate(type, key: name);

        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.Configure(c => c.Export(to).As(from));
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.Configure(c => c.Export(to).AsKeyed(from, name));
            return this;
        }

        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFactory(factoryMethod).As(type));
            return this;
        }

        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFuncWithContext((scope, staticContext, injectionContext) => factoryMethod(scope.Locate<IContainerProvider>())).As(type));
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.Configure(c => c.ExportInstance(instance).As(type).Lifestyle.Singleton());
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.Configure(c => c.ExportInstance(instance).AsKeyed(type, name).Lifestyle.Singleton());
            return this;
        }

        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            foreach (var service in serviceTypes)
                Instance.Configure(c => c.Export(type).As(service));

            return this;
        }

        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            //Instance.Configure(c => c.Export(type).As().Lifestyle.Singleton());
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            Instance.Configure(c => c.Export(to).As(from).Lifestyle.SingletonPerScope());
            return this;
        }

        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFactory(factoryMethod).As(type).Lifestyle.SingletonPerScope());
            return this;
        }

        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFuncWithContext((scope, staticContext, injectionContext) => factoryMethod(scope.Locate<IContainerProvider>())).As(type).Lifestyle.SingletonPerScope());
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.Configure(c => c.Export(to).As(from).Lifestyle.Singleton());
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Configure(c => c.Export(to).AsKeyed(from, name).Lifestyle.Singleton());
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFactory(factoryMethod).As(type).Lifestyle.Singleton());
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Configure(c => c.ExportFuncWithContext((scope, staticContext, injectionContext) => factoryMethod(scope.Locate<IContainerProvider>())).As(type).Lifestyle.Singleton());
            return this;
        }

        public object Resolve(Type type) =>
            Resolve(type, Array.Empty<(Type, object)>());

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                return Instance.Locate(type, parameters.Select(x => x.Instance).ToArray());
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, ex, this);
            }
        }

        public object Resolve(Type type, string name) =>
            Resolve(type, name, Array.Empty<(Type Type, object Instance)>());

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                return Instance.Locate(type, parameters.Select(x => x.Instance).ToArray(), withKey: name);
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, name, ex, this);
            }
        }

        Type IContainerInfo.GetRegistrationType(string key)
        {
            var exports = Instance.StrategyCollectionContainer.GetAllStrategies();
            var matchingRegistration = exports.FirstOrDefault(x => x.ExportAsName.Any(k => k.Equals(key, StringComparison.InvariantCulture)));
            //var matchingRegistration = Instance.GetServiceRegistrations().Where(r => key.Equals(r.OptionalServiceKey?.ToString(), StringComparison.Ordinal)).FirstOrDefault();
            if (matchingRegistration.ExportAs.FirstOrDefault() == null)
                return matchingRegistration.ExportAs.FirstOrDefault();
            //    matchingRegistration = Instance.GetServiceRegistrations().Where(r => key.Equals(r.ImplementationType.Name, StringComparison.Ordinal)).FirstOrDefault();

            //return matchingRegistration.ImplementationType;
            throw new NotImplementedException();
        }

        Type IContainerInfo.GetRegistrationType(Type serviceType)
        {
            //var registration = Instance.GetServiceRegistrations().Where(x => x.ServiceType == serviceType).FirstOrDefault();
            //return registration.ServiceType is null ? null : registration.ImplementationType;
            throw new NotImplementedException();
        }

        private class GraceScopedContainerProvider : IScopedProvider
        {
            private bool disposedValue;
            private IInjectionScope _scope;
            private IContainerExtension _root;

            public GraceScopedContainerProvider(IInjectionScope scope, IContainerExtension root)
            {
                _scope = scope;
                _root = root;
            }

            public bool IsAttached { get; set; }
            public IScopedProvider CurrentScope { get; }

            public IScopedProvider CreateScope() => _root.CreateScope();


            public object Resolve(Type type) =>
            Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    return _scope.Locate(type, parameters.Select(x => x.Instance).ToArray());
                }
                catch (Exception ex)
                {
                    return new ContainerResolutionException(type, ex, this);
                }
            }

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type Type, object Instance)>());

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    return _scope.Locate(type, parameters.Select(x => x.Instance).ToArray(), withKey: name);
                }
                catch (Exception ex)
                {
                    return new ContainerResolutionException(type, name, ex, this);
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _scope?.Dispose();
                    }

                    _scope = null;
                    _root = null;
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
