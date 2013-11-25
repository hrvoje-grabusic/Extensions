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
        #region .ctor
        private ContainerBuilder _builder;
        private IContainer _container;

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
                    _container = _builder.Build();
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
                    HttpContext.Current.Items[typeof(ILifetimeScope)] = value;
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
            var componentContext = GetComponentContext();
            if (!string.IsNullOrEmpty(key))
            {
                return componentContext.ResolveNamed<T>(key, ConvertParameters(parameters));
            }
            return componentContext.Resolve<T>(ConvertParameters(parameters));
        }

        public virtual object Resolve(Type type, string key = "", params Parameter[] parameters)
        {
            var componentContext = GetComponentContext();
            if (!string.IsNullOrEmpty(key))
            {
                return componentContext.ResolveNamed(key, type, ConvertParameters(parameters));
            }
            return componentContext.Resolve(type, ConvertParameters(parameters));
        }
        #endregion

        private IComponentContext GetComponentContext()
        {
            IComponentContext componentContext = this.Container;
            if (WorkUnitScope != null)
            {
                componentContext = WorkUnitScope;
            }
            return new ComponentContextWrapper(componentContext, _resolvingObjservers);
        }

        #region ResolveAll
        public virtual T[] ResolveAll<T>(string key = "")
        {
            var componentContext = GetComponentContext();
            if (!string.IsNullOrEmpty(key))
            {
                return componentContext.ResolveNamed<IEnumerable<T>>(key).ToArray();
            }
            return componentContext.Resolve<IEnumerable<T>>().ToArray();
        }
        public virtual object[] ResolveAll(Type type, string key = "")
        {
            var stype = typeof(IEnumerable<>).MakeGenericType(type);

            var componentContext = GetComponentContext();
            if (!string.IsNullOrEmpty(key))
            {
                return ((IEnumerable<object>)componentContext.ResolveNamed(key, stype)).ToArray();
            }
            return ((IEnumerable<object>)componentContext.Resolve(stype)).ToArray();
        }
        #endregion

        #region TryResolve
        public virtual T TryResolve<T>(string key = "", params Parameter[] parameters)
        {
            var componentContext = GetComponentContext();

            T obj = default(T);
            if (!string.IsNullOrEmpty(key))
            {
                if (componentContext.IsRegisteredWithKey<T>(key))
                {
                    obj = WorkUnitScope.ResolveNamed<T>(key, ConvertParameters(parameters));
                }

            }
            else
            {
                if (componentContext.IsRegistered<T>())
                {
                    obj = WorkUnitScope.Resolve<T>(ConvertParameters(parameters));
                }
            }
            return obj;
        }

        public virtual object TryResolve(Type type, string key = "", params Parameter[] parameters)
        {
            var componentContext = GetComponentContext();

            object obj = null;
            if (!string.IsNullOrEmpty(key))
            {
                if (componentContext.IsRegisteredWithKey(key, type))
                {
                    obj = WorkUnitScope.ResolveNamed(key, type, ConvertParameters(parameters));
                }
            }
            else
            {
                if (componentContext.IsRegistered(type))
                {
                    obj = WorkUnitScope.Resolve(type, ConvertParameters(parameters));
                }
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
        }
        #endregion
    }

}
