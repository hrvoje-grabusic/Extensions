using Kooboo.CMS.Synchronous.SyncSiteSetting;
using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Persistence;
using Kooboo.CMS.Sites.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Synchronous
{
    public class SyncStartup : IDependencyRegistrar
    {
        public int Order
        {
            get { return 10; }
        }

        public void Register(IContainerManager containerManager, ITypeFinder typeFinder)
        {
            containerManager.AddResolvingObserver(new ResolvingObserver());
        }

        private class ResolvingObserver : IResolvingObserver
        {
            public int Order
            {
                get { return 1; }
            }

            public object OnResolved(object resolvedObject)
            {
                if (resolvedObject != null)
                {
                    // Sync site setting
                    if (resolvedObject is SiteManager)
                    {
                        ISiteProvider siteProvider = EngineContext.Current.Resolve<ISiteProvider>();
                        IRepositoryProvider repositoryProvider = EngineContext.Current.Resolve<IRepositoryProvider>();

                        SiteManagerWrapper siteManagerWrapper = new SiteManagerWrapper(siteProvider, repositoryProvider);
                        return siteManagerWrapper;
                    }
                }

                return resolvedObject;
            }
        }
    }
}
