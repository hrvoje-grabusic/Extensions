using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Persistence;

namespace Kooboo.CMS.Synchronous.SyncSiteSetting
{
    public class SiteManagerWrapper : Kooboo.CMS.Sites.Services.SiteManager
    {
        public SiteManagerWrapper(ISiteProvider siteProvider, IRepositoryProvider repositoryProvider)
            : base(siteProvider, new RepositoryManager(repositoryProvider))
        {
        }

        public override void Update(Site site)
        {
            base.Update(site);

            RecursionVisitor.Visit(this.ChildSites(site), visit => sites =>
            {
                if (sites.Any())
                {
                    foreach (Site each in sites)
                    {
                        each.Mode = site.Mode;
                        base.Update(each);

                        visit(this.ChildSites(each));
                    }
                }
            });
        }
    }
}