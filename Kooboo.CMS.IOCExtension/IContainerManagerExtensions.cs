using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.IOCExtension
{
    public static class IContainerManagerExtensions
    {
        public static void RegisterModule(this IContainerManager containerManager, IIocModule module)
        {
            module.Load(containerManager);
        }

        public static void RegisterConfig(this IContainerManager containerManager, string file)
        {
            if (File.Exists(file))
            {
                var doc = XElement.Load(file);
                RegisterConfig(containerManager, doc);
            }
        }

        public static void RegisterConfig(this IContainerManager containerManager, XElement root)
        {
            var compEles = root.Element("components").Elements("component");
            foreach (var ele in compEles)
            {
                var serviceType = Type.GetType(ele.Attribute("type").Value);
                var interfaceType = Type.GetType(ele.Attribute("service").Value);
                ComponentLifeStyle scope = ComponentLifeStyle.Transient;
                if (ele.Attribute("scope") != null)
                {
                    switch (ele.Attribute("scope").Value.ToLower())
                    {
                        case "singleton":
                            scope = ComponentLifeStyle.Singleton;
                            break;
                        case "request":
                            scope = ComponentLifeStyle.InRequestScope;
                            break;
                        case "thread":
                            scope = ComponentLifeStyle.InThreadScope;
                            break;
                        case "transiant":
                        default:
                            scope = ComponentLifeStyle.Transient;
                            break;
                    }
                }
                string name = ele.Attribute("name") == null ? null : ele.Attribute("name").Value;
                containerManager.AddComponent(interfaceType, serviceType, name, scope);
            }

            var moduleEles = root.Element("modules").Elements("module");
            foreach (var ele in moduleEles)
            {
                var moduleType = Type.GetType(ele.Attribute("type").Value);
                IIocModule module = Activator.CreateInstance(moduleType) as IIocModule;
                containerManager.RegisterModule(module);
            }
        }
    }
}
