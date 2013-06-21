using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Configuration.Internal;
using System.Collections.Specialized;

namespace Kooboo.CMS.Configuration
{
    public sealed class ConfigurationSystem : IInternalConfigSystem
    {
        private static IInternalConfigSystem clientConfigSystem;
        private static IEnumerable<IConfigurationProvider> configProviders;

        private object appSettings;
        private object connectionStrings;

        public static void Install(IEnumerable<IConfigurationProvider> providers)
        {
            FieldInfo[] fiStateValues = null;
            Type tInitState = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState", BindingFlags.NonPublic);

            if (null != tInitState)
            {
                fiStateValues = tInitState.GetFields();
            }

            FieldInfo fiInit = typeof(System.Configuration.ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fiSystem = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);

            if (fiInit != null && fiSystem != null && null != fiStateValues)
            {
                fiInit.SetValue(null, fiStateValues[1].GetValue(null));
                fiSystem.SetValue(null, null);
            }

            ConfigurationSystem confSys = new ConfigurationSystem();
            Type configFactoryType = Type.GetType("System.Configuration.Internal.InternalConfigSettingsFactory, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            IInternalConfigSettingsFactory configSettingsFactory = (IInternalConfigSettingsFactory)Activator.CreateInstance(configFactoryType, true);
            configSettingsFactory.SetConfigurationSystem(confSys, false);

            Type clientConfigSystemType = Type.GetType("System.Configuration.ClientConfigurationSystem, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            clientConfigSystem = (IInternalConfigSystem)Activator.CreateInstance(clientConfigSystemType, true);

            configProviders = providers;
            if (configProviders != null)
            {
                foreach (var p in configProviders)
                {
                    p.LoadConfigurations();
                }
            }
        }

        #region IInternalConfigSystem Members

        public object GetSection(string configKey)
        {
            object section = clientConfigSystem.GetSection(configKey);

            switch (configKey)
            {
                case "appSettings":
                    if (appSettings != null)
                        return appSettings;
                    if (section is NameValueCollection)
                    {
                        if (configProviders != null)
                        {
                            var cfg = new NameValueCollection();
                            NameValueCollection localSettings = (NameValueCollection)section;
                            foreach (string key in localSettings)
                            {
                                cfg.Add(key, localSettings[key]);
                            }
                            foreach (var configProvider in configProviders)
                            {
                                var settings = configProvider.GetAppSettings();
                                if (settings != null)
                                {
                                    foreach (string key in settings)
                                    {
                                        cfg.Add(key, settings[key]);
                                    }
                                }
                            }
                            this.appSettings = cfg;
                            section = this.appSettings;
                        }
                        else
                        {
                            this.appSettings = section;
                        }
                    }
                    break;
                case "connectionStrings":
                    if (this.connectionStrings != null)
                    {
                        return this.connectionStrings;
                    }

                    if (configProviders != null)
                    {
                        ConnectionStringsSection connectionStringsSection = new ConnectionStringsSection();
                        foreach (ConnectionStringSettings connectionStringSetting in ((ConnectionStringsSection)section).ConnectionStrings)
                        {
                            connectionStringsSection.ConnectionStrings.Add(connectionStringSetting);
                        }
                        foreach (var configProvider in configProviders)
                        {
                            var conns = configProvider.GetConnectionStrings();
                            if (conns != null)
                            {
                                foreach (ConnectionStringSettings connectionStringSetting in conns)
                                {
                                    connectionStringsSection.ConnectionStrings.Add(connectionStringSetting);
                                }
                            }
                        }
                        this.connectionStrings = connectionStringsSection;
                        section = this.connectionStrings;
                    }
                    else
                    {
                        this.connectionStrings = section;
                    }
                    break;
            }

            return section;
        }

        public void RefreshConfig(string sectionName)
        {
            if (sectionName == "appSettings" || sectionName == "connectionStrings")
            {
                if (sectionName == "appSettings")
                {
                    this.appSettings = null;
                }

                if (sectionName == "connectionStrings")
                {
                    this.connectionStrings = null;
                }

                clientConfigSystem.RefreshConfig(sectionName);
                if (configProviders != null)
                {
                    foreach (var p in configProviders)
                    {
                        p.LoadConfigurations();
                    }
                }
            }
        }

        public bool SupportsUserConfig
        {
            get { return clientConfigSystem.SupportsUserConfig; }
        }

        #endregion
    }
}
