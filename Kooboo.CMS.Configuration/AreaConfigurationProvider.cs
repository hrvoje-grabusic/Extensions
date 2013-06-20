using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.Configuration
{
    [Dependency(typeof(IConfigurationProvider), ComponentLifeStyle.Singleton)]
    public class AreaConfigurationProvider : IConfigurationProvider
    {
        private NameValueCollection appSettings;
        private ConnectionStringSettingsCollection connSettings;

        public AreaConfigurationProvider()
        {
            LoadConfigurations();
        }

        public void LoadConfigurations()
        {
            appSettings = new NameValueCollection();
            connSettings = new ConnectionStringSettingsCollection();
            var areaDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Areas"));
            if (areaDir.Exists)
            {
                foreach (var dir in areaDir.GetDirectories())
                {
                    var file = dir.GetFiles("web.config").FirstOrDefault();
                    if (file != null)
                    {
                        var config = WebConfigurationManager.OpenWebConfiguration(string.Format("/Areas/{0}/web.config", dir.Name));
                        var settings = config.AppSettings.Settings;
                        foreach (var key in settings.AllKeys)
                        {
                            appSettings.Add(key, settings[key].Value);
                        }
                        var conns = config.ConnectionStrings.ConnectionStrings;
                        foreach (ConnectionStringSettings conn in conns)
                        {
                            ConnectionStringSettings c = new ConnectionStringSettings(conn.Name, conn.ConnectionString, conn.ProviderName);
                            connSettings.Add(c);
                        }
                    }
                }
            }
        }

        public NameValueCollection GetAppSettings()
        {
            return appSettings;
        }

        public ConnectionStringSettingsCollection GetConnectionStrings()
        {
            return connSettings;
        }
    }
}
