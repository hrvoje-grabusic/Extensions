using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    public class ComponentContextWrapper : IComponentContext
    {
        IComponentContext _context;
        IEnumerable<IResolvingObserver> _observers;
        public ComponentContextWrapper(IComponentContext context, IEnumerable<IResolvingObserver> observers)
        {
            _context = context;
            _observers = observers;
        }
        public global::Autofac.Core.IComponentRegistry ComponentRegistry
        {
            get { return new ComponentRegistryWrapper(_context.ComponentRegistry, _observers); }
        }

        public object ResolveComponent(global::Autofac.Core.IComponentRegistration registration, IEnumerable<global::Autofac.Core.Parameter> parameters)
        {
            return _context.ResolveComponent(registration, parameters);
        }
    }
}
