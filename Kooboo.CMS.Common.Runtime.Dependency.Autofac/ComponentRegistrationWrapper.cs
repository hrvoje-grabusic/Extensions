using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    class DecoratorInstanceActivator : IInstanceActivator
    {
        IInstanceActivator _actualActivator;
        IEnumerable<IResolvingObserver> _observers;
        public DecoratorInstanceActivator(IInstanceActivator actualActivator, IEnumerable<IResolvingObserver> observers)
        {
            _actualActivator = actualActivator;
            _observers = observers;
        }
        public object ActivateInstance(global::Autofac.IComponentContext context, IEnumerable<global::Autofac.Core.Parameter> parameters)
        {
            context = new ComponentContextWrapper(context, _observers);

            var obj = _actualActivator.ActivateInstance(context, parameters);

            return OnResolved(obj);
        }
        private object OnResolved(object resolvedObject)
        {
            if (_observers != null)
            {
                foreach (var item in _observers)
                {
                    resolvedObject = item.OnResolved(resolvedObject);
                }
            }
            return resolvedObject;
        }
        public Type LimitType
        {
            get { return _actualActivator.LimitType; }
        }

        public void Dispose()
        {
            _actualActivator.Dispose();
        }
    }
    public class ComponentRegistrationWrapper : IComponentRegistration
    {
        IComponentRegistration _componentRegistration;
        IEnumerable<IResolvingObserver> _observers;
        public ComponentRegistrationWrapper(IComponentRegistration componentRegistration, IEnumerable<IResolvingObserver> observers)
        {
            this._componentRegistration = componentRegistration;
            this._observers = observers;
        }
        public event EventHandler<ActivatedEventArgs<object>> Activated
        {
            add
            {
                this._componentRegistration.Activated += value;
            }
            remove
            {
                this._componentRegistration.Activated -= value;
            }
        }

        public event EventHandler<ActivatingEventArgs<object>> Activating
        {
            add
            {
                this._componentRegistration.Activating += value;
            }
            remove
            {
                this._componentRegistration.Activating -= value;
            }
        }


        public IInstanceActivator Activator
        {
            get { return new DecoratorInstanceActivator(_componentRegistration.Activator, _observers); }
        }

        public Guid Id
        {
            get { return _componentRegistration.Id; }
        }

        public IComponentLifetime Lifetime
        {
            get { return _componentRegistration.Lifetime; }
        }

        public IDictionary<string, object> Metadata
        {
            get { return _componentRegistration.Metadata; }
        }

        public InstanceOwnership Ownership
        {
            get { return _componentRegistration.Ownership; }
        }

        public event EventHandler<PreparingEventArgs> Preparing
        {
            add
            {
                this._componentRegistration.Preparing += value;
            }
            remove
            {
                this._componentRegistration.Preparing -= value;
            }
        }


        public void RaiseActivated(global::Autofac.IComponentContext context, IEnumerable<global::Autofac.Core.Parameter> parameters, object instance)
        {
            _componentRegistration.RaiseActivated(context, parameters, instance);
        }

        public void RaiseActivating(global::Autofac.IComponentContext context, IEnumerable<global::Autofac.Core.Parameter> parameters, ref object instance)
        {
            _componentRegistration.RaiseActivating(context, parameters, ref instance);
        }

        public void RaisePreparing(global::Autofac.IComponentContext context, ref IEnumerable<global::Autofac.Core.Parameter> parameters)
        {
            _componentRegistration.RaisePreparing(context, ref parameters);
        }

        public IEnumerable<Service> Services
        {
            get { return _componentRegistration.Services; }
        }

        public InstanceSharing Sharing
        {
            get { return _componentRegistration.Sharing; }
        }

        public IComponentRegistration Target
        {
            get { return _componentRegistration.Target; }
        }

        public void Dispose()
        {
            _componentRegistration.Dispose();
        }
    }
}
