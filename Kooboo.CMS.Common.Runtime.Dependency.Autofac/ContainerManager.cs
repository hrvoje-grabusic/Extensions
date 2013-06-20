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
        public virtual void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent<TService, TService>(key, lifeStyle);
        }

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="lifeStyle">The life style.</param>
        public virtual void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
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
        public virtual void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            AddComponent(typeof(TService), typeof(TImplementation), key, lifeStyle);
        }

        public virtual void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            var rb = _builder.RegisterType(implementation).As(service);
            if (!string.IsNullOrEmpty(key))
                rb.Named(key, service);
            switch (lifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    rb.SingleInstance();
                    break;
                case ComponentLifeStyle.InRequestScope:
                    rb.InstancePerMatchingLifetimeScope("httpRequest");
                    break;
                case ComponentLifeStyle.Transient:
                default:
                    rb.InstancePerDependency();
                    break;
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

        #region Resolve
        public virtual T Resolve<T>(string key = "") where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered<T>())
                    return WorkUnitScope.Resolve<T>();
                return Container.Resolve<T>();
            }
            return Container.ResolveNamed<T>(key);
        }

        public virtual object Resolve(Type type, string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered(type))
                    return WorkUnitScope.Resolve(type);
                return Container.Resolve(type);
            }
            return Container.ResolveNamed(key, type);
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
        public virtual T TryResolve<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                T obj = default(T);
                if (WorkUnitScope != null && WorkUnitScope.IsRegistered<T>())
                    obj = WorkUnitScope.Resolve<T>();
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

        public virtual object TryResolve(Type type, string key = "")
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
    }

}
