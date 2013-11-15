#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Features.ResolveAnything;
namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    /// <summary>
    /// 加入Container Manager是为了减少其它程序在做注入的时候减少对Autofac的依赖
    /// 有了这个类以后，未来的扩展程序集在注册组件时就不用引用Autofac，对它形成依赖。
    /// </summary>
    public class ContainerManager : IContainerManager
    {
        #region ContainerWrapper
        private class ContainerWrapper : IContainer
        {

            IContainer _container;
            public ContainerWrapper(IContainer container, IEnumerable<IResolvingObserver> observers)
            {
                this._container = container;
                this._resolvingObjservers.AddRange(observers);
            }
            #region AddResolvingObserver

            private List<IResolvingObserver> _resolvingObjservers = new List<IResolvingObserver>();
            public void AddResolvingObserver(IResolvingObserver observer)
            {
                _resolvingObjservers.Add(observer);
                _resolvingObjservers = _resolvingObjservers.OrderBy(it => it.Order).ToList();
            }

            private object OnResolved(object resolvedObject)
            {
                if (_resolvingObjservers.Count > 0)
                {
                    foreach (var item in _resolvingObjservers)
                    {
                        resolvedObject = item.OnResolved(resolvedObject);
                    }
                }
                return resolvedObject;
            }
            #endregion

            #region IContainer
            public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
            {
                return _container.BeginLifetimeScope(tag, configurationAction);
            }

            public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
            {
                return _container.BeginLifetimeScope(configurationAction);
            }

            public ILifetimeScope BeginLifetimeScope(object tag)
            {
                return _container.BeginLifetimeScope(tag);
            }

            public ILifetimeScope BeginLifetimeScope()
            {
                return _container.BeginLifetimeScope();
            }

            public event EventHandler<global::Autofac.Core.Lifetime.LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning
            {
                add
                {
                    _container.ChildLifetimeScopeBeginning += value;
                }
                remove
                {
                    _container.ChildLifetimeScopeBeginning -= value;
                }
            }

            public event EventHandler<global::Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs> CurrentScopeEnding
            {
                add
                {
                    _container.CurrentScopeEnding += value;
                }
                remove
                {
                    _container.CurrentScopeEnding -= value;
                }
            }

            public global::Autofac.Core.IDisposer Disposer
            {
                get { return _container.Disposer; }
            }

            public event EventHandler<global::Autofac.Core.Resolving.ResolveOperationBeginningEventArgs> ResolveOperationBeginning
            {
                add
                {
                    _container.ResolveOperationBeginning += value;
                }
                remove
                {
                    _container.ResolveOperationBeginning -= value;
                }
            }

            public object Tag
            {
                get { return _container.Tag; }
            }

            public global::Autofac.Core.IComponentRegistry ComponentRegistry
            {
                get { return _container.ComponentRegistry; }
            }

            public object ResolveComponent(global::Autofac.Core.IComponentRegistration registration, IEnumerable<global::Autofac.Core.Parameter> parameters)
            {
                registration.Activating += registration_Activating;
                var o = _container.ResolveComponent(registration, parameters);
                return OnResolved(o);
            }

            void registration_Activating(object sender, global::Autofac.Core.ActivatingEventArgs<object> e)
            {
                e.Instance=OnResolved(e.Instance);
            }

            public void Dispose()
            {
                _container.Dispose();
            }
            #endregion
        }
        #endregion

        #region LifetimeScopeWrapper

        private class LifetimeScopeWrapper : ILifetimeScope
        {
            public LifetimeScopeWrapper(ILifetimeScope lifetimeScope, IEnumerable<IResolvingObserver> observers)
            {
                this.lifetimeScope = lifetimeScope;
                _resolvingObjservers.AddRange(observers);
            }
            private ILifetimeScope lifetimeScope;
            private List<IResolvingObserver> _resolvingObjservers = new List<IResolvingObserver>();
            private object OnResolved(object resolvedObject)
            {
                if (_resolvingObjservers.Count > 0)
                {
                    foreach (var item in _resolvingObjservers)
                    {
                        resolvedObject = item.OnResolved(resolvedObject);
                    }
                }
                return resolvedObject;
            }
            public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
            {
                return this.lifetimeScope.BeginLifetimeScope(tag, configurationAction);
            }

            public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
            {
                return this.lifetimeScope.BeginLifetimeScope(configurationAction);
            }

            public ILifetimeScope BeginLifetimeScope(object tag)
            {
                return this.lifetimeScope.BeginLifetimeScope(tag);
            }

            public ILifetimeScope BeginLifetimeScope()
            {
                return this.lifetimeScope.BeginLifetimeScope();
            }

            public event EventHandler<global::Autofac.Core.Lifetime.LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning {
                add
                {
                    this.lifetimeScope.ChildLifetimeScopeBeginning += value;
                }
                remove
                {
                    this.lifetimeScope.ChildLifetimeScopeBeginning -= value;
                }
            }

            public event EventHandler<global::Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs> CurrentScopeEnding {
                add
                {
                    this.lifetimeScope.CurrentScopeEnding += value;
                }
                remove
                {
                    this.lifetimeScope.CurrentScopeEnding -= value;
                }
            }

            public global::Autofac.Core.IDisposer Disposer
            {
                get { return this.lifetimeScope.Disposer; }
            }

            public event EventHandler<global::Autofac.Core.Resolving.ResolveOperationBeginningEventArgs> ResolveOperationBeginning
            {
                add
                {
                    this.lifetimeScope.ResolveOperationBeginning += value;
                }
                remove
                {
                    this.lifetimeScope.ResolveOperationBeginning -= value;
                }
            }

            public object Tag
            {
                get { return this.lifetimeScope.Tag; }
            }

            public global::Autofac.Core.IComponentRegistry ComponentRegistry
            {
                get { return this.lifetimeScope.ComponentRegistry; }
            }

            public object ResolveComponent(global::Autofac.Core.IComponentRegistration registration, IEnumerable<global::Autofac.Core.Parameter> parameters)
            {
                registration.Activating += registration_Activating;
                var o = this.lifetimeScope.ResolveComponent(registration, parameters);
                return OnResolved(o);
            }

            void registration_Activating(object sender, global::Autofac.Core.ActivatingEventArgs<object> e)
            {
                e.Instance = OnResolved(e.Instance);
            }

            public void Dispose()
            {
                this.lifetimeScope.Dispose();
            }
        }
        #endregion
        #region .ctor

        private ContainerBuilder _builder;
        private ContainerWrapper _container;

        public ContainerManager()
        {
            _builder = new ContainerBuilder();
        }

        /// <summary>
        /// ContainerBuilder
        /// </summary>
        public ContainerBuilder Builder
        {
            get
            {
                return _builder;
            }
        }

        /// <summary>
        /// IContainer
        /// </summary>
        public IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
                    _container = new ContainerWrapper(_builder.Build(), _resolvingObjservers);
                }
                return _container;
            }
        }

        internal ILifetimeScope WorkUnitScope
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Items[typeof(ILifetimeScope)] as ILifetimeScope;
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[typeof(ILifetimeScope)] = new LifetimeScopeWrapper(value, this._resolvingObjservers);
            }
        }

        #endregion

        #region AddComponent

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lifeStyle">The life style.</param>
        public virtual void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Transient)
        {
            AddComponent<TService, TService>(key, lifeStyle);
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="lifeStyle">The life style.</param>
        public virtual void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Transient)
        {
            AddComponent(service, service, key, lifeStyle);
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lifeStyle">The life style.</param>
        public virtual void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Transient)
        {
            AddComponent(typeof(TService), typeof(TImplementation), key, lifeStyle);
        }

        public virtual void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Transient, params Parameter[] parameters)
        {
            if (service.IsGenericTypeDefinition)
            {
                var rb = _builder.RegisterGeneric(implementation).As(service).UsingConstructor(new ParameterlessConstructorSelector());
                rb = rb.WithParamterEx(parameters);
                if (!string.IsNullOrEmpty(key))
                    rb = rb.Named(key, service);
                rb = rb.LifeStyle(lifeStyle);

            }
            else
            {
                var rb = _builder.RegisterType(implementation).As(service).UsingConstructor(new ParameterlessConstructorSelector());
                rb = rb.As(service);
                rb = rb.WithParamterEx(parameters);

                if (!string.IsNullOrEmpty(key))
                    rb = rb.Named(key, service);
                rb = rb.LifeStyle(lifeStyle);
            }
        }

        public virtual void AddComponentInstance<TService>(object instance, string key = "")
        {
            AddComponentInstance(typeof(TService), instance, key);
        }
        public virtual void AddComponentInstance(object instance, string key = "")
        {
            AddComponentInstance(instance.GetType(), instance, key);
        }
        public virtual void AddComponentInstance(Type service, object instance, string key = "")
        {
            var rb = _builder.RegisterInstance(instance).As(service);
            if (!string.IsNullOrEmpty(key))
                rb.Named(key, service);
        }
        #endregion

        #region ConvertParameters
        private static global::Autofac.Core.Parameter[] ConvertParameters(Parameter[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            return parameters.Select(it => new NamedParameter(it.Name, it.ValueCallback())).ToArray();
        }
        #endregion

        #region Resolve
        public virtual T Resolve<T>(string key = "", params Parameter[] parameters) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered<T>())
                    return WorkUnitScope.Resolve<T>();
                return Container.Resolve<T>(ConvertParameters(parameters));
            }
            return Container.ResolveNamed<T>(key, ConvertParameters(parameters));
        }

        public virtual object Resolve(Type type, string key = "", params Parameter[] parameters)
        {
            if (string.IsNullOrEmpty(key))
            {
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered(type))
                    return WorkUnitScope.Resolve(type, ConvertParameters(parameters));
                return Container.Resolve(type, ConvertParameters(parameters));
            }
            return Container.ResolveNamed(key, type, ConvertParameters(parameters));
        }
        #endregion

        #region ResolveAll
        public virtual T[] ResolveAll<T>(string key = "")
        {
            if (WorkUnitScope != null && WorkUnitScope.IsRegistered<IEnumerable<T>>())
                return WorkUnitScope.Resolve<IEnumerable<T>>().ToArray();
            return Container.Resolve<IEnumerable<T>>().ToArray();
        }
        public virtual object[] ResolveAll(Type type, string key = "")
        {
            var stype = typeof(IEnumerable<>).MakeGenericType(type);
            if (WorkUnitScope != null && WorkUnitScope.IsRegistered(type))
                return ((IEnumerable<object>)WorkUnitScope.Resolve(type)).ToArray();
            return ((IEnumerable<object>)Container.Resolve(stype)).ToArray();
        }
        #endregion

        #region TryResolve
        public virtual T TryResolve<T>(string key = "", params Parameter[] parameters)
        {
            if (string.IsNullOrEmpty(key))
            {
                T obj = default(T);
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered<T>())
                    obj = WorkUnitScope.Resolve<T>(ConvertParameters(parameters));
                else
                    Container.TryResolve<T>(out obj);
                return obj;
            }
            else
            {
                object obj = null;
                if (Container.TryResolveNamed(key, typeof(T), out obj))
                {
                    return (T)obj;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public virtual object TryResolve(Type type, string key = "", params Parameter[] parameters)
        {
            object obj = null;
            if (string.IsNullOrEmpty(key))
            {
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered(type))
                    obj = WorkUnitScope.Resolve(type);
                else
                    Container.TryResolve(type, out obj);
            }
            else
            {
                Container.TryResolveNamed(key, type, out obj);
            }
            return obj;
        }

        #endregion

        #region ResolveUnregistered
        public virtual T ResolveUnregistered<T>() where T : class
        {
            return ResolveUnregistered(typeof(T)) as T;
        }

        public virtual object ResolveUnregistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var parameterInstances = new List<object>();
                foreach (var parameter in parameters)
                {
                    var service = Resolve(parameter.ParameterType);
                    if (service == null)
                        parameterInstances.Add(service);
                }
                return Activator.CreateInstance(type, parameterInstances.ToArray());


            }
            throw new Exception("No contructor was found that had all the dependencies satisfied.");
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (this._container != null)
            {
                this._container.Dispose();
            }

            this._container = null;
        }
        #endregion

        #region AddResolvingObserver
        private IList<IResolvingObserver> _resolvingObjservers = new List<IResolvingObserver>();
        public void AddResolvingObserver(IResolvingObserver observer)
        {
            _resolvingObjservers.Add(observer);
            if (_container != null)
            {
                _container.AddResolvingObserver(observer);
            }
        }
        #endregion
    }

}
