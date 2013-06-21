using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Common;
using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.Configuration
{
    [Dependency(typeof(IHttpApplicationEvents), ComponentLifeStyle.Singleton, Key = "ConfigurationAppHandler")]
    public class ConfigurationAppHandler : IHttpApplicationEvents
    {
        public void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        public void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        public void Application_End(object sender, EventArgs e)
        {
        }

        public void Application_EndRequest(object sender, EventArgs e)
        {
        }

        public void Application_Error(object sender, EventArgs e)
        {
        }

        public void Application_Start(object sender, EventArgs e)
        {
            var cps = EngineContext.Current.ResolveAll<IConfigurationProvider>();
            if(cps != null && cps.Count() > 0)
                ConfigurationSystem.Install(cps);
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
