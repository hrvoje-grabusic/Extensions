using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.CMS.IOCExtension
{
    public interface IIocModule
    {
        void Load(IContainerManager containerManager);
    }
}
