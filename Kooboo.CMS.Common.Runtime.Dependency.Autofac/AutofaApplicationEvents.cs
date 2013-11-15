using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kooboo.CMS.Common;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    [Dependency(typeof(IHttpApplicationEvents), ComponentLifeStyle.Singleton, Key = "AutofaApplicationEvents")]
    public class AutofaApplicationEvents : IHttpApplicationEvents
    {
        public void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        public void Application_BeginRequest(object sender, EventArgs e)
        {
            if (EngineContext.Current is AutofacEngine)
            {
                var cm = ((AutofacEngine)EngineContext.Current).ContainerManager as ContainerManager;
                if (cm != null)
                {
                    cm.WorkUnitScope = cm.Container.BeginLifetimeScope("httpRequest");
                    cm.WorkUnitScope.CurrentScopeEnding += WorkUnitScope_CurrentScopeEnding;

                }
            }
        }

        void WorkUnitScope_CurrentScopeEnding(object sender, global::Autofac.Core.Lifetime.LifetimeScopeEndingEventArgs e)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[typeof(ILifetimeScope)] = null;
        }

        public void Application_End(object sender, EventArgs e)
        {
        }

        public void Application_EndRequest(object sender, EventArgs e)
        {
            if (EngineContext.Current is AutofacEngine)
            {
                var cm = ((AutofacEngine)EngineContext.Current).ContainerManager as ContainerManager;
                if (cm != null && cm.WorkUnitScope != null)
                {
                    cm.WorkUnitScope.Dispose();
                    cm.WorkUnitScope = null;
                }
            }
        }

        public void Application_Error(object sender, EventArgs e)
        {
        }

        public void Application_Start(object sender, EventArgs e)
        {

        }

        public void Init(System.Web.HttpApplication httpApplication)
        {
        }

        public void Session_End(object sender, EventArgs e)
        {
        }

        public void Session_Start(object sender, EventArgs e)
        {
        }
    }
}
