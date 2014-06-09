using Kooboo.CMS.Synchronous.SyncContentSchema;
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
                    // Sync content schema
                    if (resolvedObject is ISchemaProvider)
                    {
                        ISchemaProvider schemaProvider = (ISchemaProvider)resolvedObject;
                        ISiteProvider siteProvider = EngineContext.Current.Resolve<ISiteProvider>();

                        return new SchemaProviderWrapper(schemaProvider, siteProvider);
                    }

                    if (resolvedObject is SchemaManager)
                    {
                        ISchemaProvider schemaProvider = EngineContext.Current.Resolve<ISchemaProvider>();
                        SchemaManager schemaManager = (SchemaManager)resolvedObject;
                        ISiteProvider siteProvider = EngineContext.Current.Resolve<ISiteProvider>();

                        return new SchemaManagerWrapper(schemaProvider, schemaManager, siteProvider);
                    }
                }

                return resolvedObject;
            }
        }
    }
}
