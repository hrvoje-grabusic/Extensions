using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    public class ComponentRegistryWrapper : IComponentRegistry
    {
        IComponentRegistry _componentRegistry;
        IEnumerable<IResolvingObserver> _observers;
        public ComponentRegistryWrapper(IComponentRegistry componentRegistry, IEnumerable<IResolvingObserver> observers)
        {
            this._componentRegistry = componentRegistry;
            this._observers = observers;
        }
        public void AddRegistrationSource(IRegistrationSource source)
        {
            _componentRegistry.AddRegistrationSource(source);
        }

        public bool HasLocalComponents
        {
            get { return this._componentRegistry.HasLocalComponents; }
        }

        public bool IsRegistered(Service service)
        {
            return this._componentRegistry.IsRegistered(service);
        }

        public void Register(IComponentRegistration registration, bool preserveDefaults)
        {
            this._componentRegistry.Register(registration, preserveDefaults);
        }

        public void Register(IComponentRegistration registration)
        {
            this._componentRegistry.Register(registration);
        }

        public event EventHandler<ComponentRegisteredEventArgs> Registered
        {
            add
            {
                this._componentRegistry.Registered += value;
            }
            remove
            {
                this._componentRegistry.Registered -= value;
            }
        }

        public event EventHandler<RegistrationSourceAddedEventArgs> RegistrationSourceAdded
        {
            add
            {
                this._componentRegistry.RegistrationSourceAdded += value;
            }
            remove
            {
                this._componentRegistry.RegistrationSourceAdded -= value;
            }
        }

        public IEnumerable<IComponentRegistration> Registrations
        {
            get { return this._componentRegistry.Registrations; }
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service)
        {
            return this._componentRegistry.RegistrationsFor(service);
        }

        public IEnumerable<IRegistrationSource> Sources
        {
            get { return this._componentRegistry.Sources; }
        }

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            var ret = this._componentRegistry.TryGetRegistration(service, out registration);

            if (ret && registration != null)
            {
                registration = new ComponentRegistrationWrapper(registration, _observers);
            }
            return ret;
        }

        public void Dispose()
        {
            this._componentRegistry.Dispose();
        }
    }
}
