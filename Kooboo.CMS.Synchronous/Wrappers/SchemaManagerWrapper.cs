using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Persistence;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Sites.Models;

namespace Kooboo.CMS.Synchronous.SyncContentSchema
{
    public class SchemaManagerWrapper : SchemaManager
    {
        private readonly SchemaManager _schemaManager;
        private readonly ISiteProvider _siteProvider;

        public SchemaManagerWrapper(ISchemaProvider schemaProvider, SchemaManager schemaManager, ISiteProvider siteProvider)
            : base(schemaProvider)
        {
            _schemaManager = schemaManager;
            _siteProvider = siteProvider;
        }

        public override void ResetForm(Repository orginalRepository, string schemaName, FormType formType)
        {
            _schemaManager.ResetForm(orginalRepository, schemaName, formType);

            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                Schema targetSchema = _schemaManager.Get(targetRepository, schemaName);
                if (targetSchema != null)
                {
                    _schemaManager.ResetForm(targetRepository, schemaName, formType);
                }
            });
        }

        private void Sync(Site site, Repository orginalRepository, Action<Repository> syncAction)
        {
            if (site == null)
            {
                return;
            }

            Repository siteRepository = site.GetRepository();
            if (siteRepository != orginalRepository)
            {
                syncAction(siteRepository);
            }

            foreach (Site childSite in _siteProvider.ChildSites(site))
            {
                Sync(childSite, siteRepository, syncAction);
            }
        }
    }
}