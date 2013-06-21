using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Kooboo.CMS.Configuration
{
    public interface IConfigurationProvider
    {
        void LoadConfigurations();
        NameValueCollection GetAppSettings();
        ConnectionStringSettingsCollection GetConnectionStrings();
    }
}
