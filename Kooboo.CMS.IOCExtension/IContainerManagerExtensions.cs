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

                if (ele.HasElements)
                {
                    var paraEles = ele.Element("parameters").Elements("parameter");
                    var constructors = serviceType.GetConstructors();
                    foreach (var constructor in constructors)
                    {
                        var parameters = constructor.GetParameters();
                        List<object> pvals = new List<object>();
                        try
                        {
                            foreach (var para in parameters)
                            {
                                var pele = paraEles.FirstOrDefault(e => e.Attribute("name").Value.ToLower() == para.Name.ToLower());
                                object val = null;
                                if (pele != null)
                                {
                                    val = ConvertTo(para.ParameterType, pele.Attribute("value").Value);
                                }
                                else
                                {
                                    val = containerManager.TryResolve(para.ParameterType);
                                }
                                pvals.Add(val);
                            }

                            object instance = constructor.Invoke(pvals.ToArray());
                            containerManager.AddComponentInstance(interfaceType, instance, name);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    containerManager.AddComponent(interfaceType, serviceType, name, scope);
                }
            }

            var moduleEles = root.Element("modules").Elements("module");
            foreach (var ele in moduleEles)
            {
                var moduleType = Type.GetType(ele.Attribute("type").Value);
                IIocModule module = Activator.CreateInstance(moduleType) as IIocModule;
                containerManager.RegisterModule(module);
            }
        }

        private static object ConvertTo(Type type, string value)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else if (typeof(Guid).IsAssignableFrom(type))
            {
                Guid guid = new Guid(value);
                return guid as object;
            }
            else if (typeof(bool).IsAssignableFrom(type))
            {
                bool val = false;
                switch (value.ToLower())
                {
                    case "on":
                    case "yes":
                    case "1":
                    case "true":
                        val = true;
                        break;
                    default:
                        val = false;
                        break;
                }
                return val as object;
            }
            else if (typeof(System.Xml.Linq.XElement).IsAssignableFrom(type))
            {
                return System.Xml.Linq.XElement.Parse(value);
            }
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch { }
            }

            throw new NotSupportedException(string.Format("Cannot convert {0} to type {1}", value, type.FullName));
        }
    }
}
