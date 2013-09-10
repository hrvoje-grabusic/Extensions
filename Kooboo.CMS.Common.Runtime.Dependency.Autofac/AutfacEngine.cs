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
using System.Configuration;
using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    public class AutofacEngine : IEngine
    {
        #region Ctor

        public AutofacEngine()
            : this(new WebAppTypeFinder() { AssemblySkipLoadingPattern = "Kooboo," })
        {

        }
        public AutofacEngine(ITypeFinder typeFinder)
            : this(typeFinder, new ContainerManager())
        { }
        public AutofacEngine(ITypeFinder typeFinder, ContainerManager containerManager)
        {
            if (typeFinder == null)
            {
                throw new ArgumentNullException("typeFinder");
            }
            this.TypeFinder = typeFinder;
            this.ContainerManager = containerManager;
            InitializeContainer();
        }

        #endregion

        #region Utilities

        private void RunStartupTasks()
        {
            var startUpTaskTypes = this.TypeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        private void InitializeContainer()
        {
            //register attributed dependency
            var attrDependency = new DependencyAttributeRegistrator(this.TypeFinder, this.ContainerManager);
            attrDependency.RegisterServices();

            //
            var drTypes = this.TypeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(this.ContainerManager, this.TypeFinder);

            // register controllers
            ((ContainerManager)this.ContainerManager).Builder.RegisterControllers(this.TypeFinder.GetAssemblies().ToArray());

            //System.Web.Mvc.DependencyResolver.SetResolver(new AutofacDependencyResolver(((ContainerManager)this.ContainerManager).Container));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize()
        {
            RunStartupTasks();
        }

        #region Resolve
        public T Resolve<T>(params Parameter[] parameters) where T : class
        {
            return ContainerManager.Resolve<T>(null, parameters);
        }

        public T Resolve<T>(string name, params Parameter[] parameters) where T : class
        {
            return ContainerManager.Resolve<T>(name, parameters);
        }

        public object Resolve(Type type, string name, params Parameter[] parameters)
        {
            return ContainerManager.Resolve(type, name, parameters);
        }

        public object Resolve(Type type, params Parameter[] parameters)
        {
            return ContainerManager.Resolve(type, null, parameters);
        }
        #endregion

        #region TryResolve
        public T TryResolve<T>(params Parameter[] parameters) where T : class
        {
            return ContainerManager.TryResolve<T>(null, parameters);
        }

        public T TryResolve<T>(string name, params Parameter[] parameters) where T : class
        {
            return ContainerManager.TryResolve<T>(name, parameters);
        }

        public object TryResolve(Type type, string name, params Parameter[] parameters)
        {
            return ContainerManager.TryResolve(type, name, parameters);
        }

        public object TryResolve(Type type, params Parameter[] parameters)
        {
            return ContainerManager.TryResolve(type, null, parameters);
        }
        #endregion

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return ContainerManager.ResolveAll(serviceType);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public IContainerManager ContainerManager
        {
            get;
            private set;
        }
        public ITypeFinder TypeFinder { get; private set; }
        #endregion
    }
}
