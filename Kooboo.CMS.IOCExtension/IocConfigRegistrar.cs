using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.IOCExtension
{
    public class IocConfigRegistrar : IDependencyRegistrar
    {
        public void Register(IContainerManager containerManager, ITypeFinder typeFinder)
        {
            var areaDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Areas"));
            if (areaDir.Exists)
            {
                var iocXmlFiles = areaDir.GetFiles("ioc.config", SearchOption.AllDirectories);
                foreach (var iocXmlFile in iocXmlFiles)
                {
                    containerManager.RegisterConfig(iocXmlFile.FullName);
                }
            }
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
