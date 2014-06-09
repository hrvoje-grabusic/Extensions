using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Persistence;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;

namespace Kooboo.CMS.Synchronous.SyncContentFolder
{
    public class TextFolderManagerWrapper : TextFolderManager
    {
        private readonly TextFolderManager _textFolderManager;
        private readonly ISiteProvider _siteProvider;

        public TextFolderManagerWrapper(ITextFolderProvider textFolderProvider, TextFolderManager textFolderManger,
            ISiteProvider siteProvider)
            : base(textFolderProvider)
        {
            _textFolderManager = textFolderManger;
            _siteProvider = siteProvider;
        }

        public override void Add(Repository orginalRepository, TextFolder folder)
        {
            _textFolderManager.Add(orginalRepository, folder);

            Sync(Site.Current, orginalRepository, (Repository targetRepository) =>
            {
                EnsureFolder(targetRepository, folder);
                _textFolderManager.Add(targetRepository, folder);
            });
        }

        public override void Update(Repository orginalRepository, TextFolder newFolder, TextFolder oldFolder)
        {
            _textFolderManager.Update(orginalRepository, newFolder, oldFolder);

            Sync(Site.Current, orginalRepository, (Repository targetRepository) =>
            {
                EnsureFolder(targetRepository, newFolder);
                EnsureFolder(targetRepository, oldFolder);
                _textFolderManager.Update(targetRepository, newFolder, oldFolder);
            });
        }

        public override void Remove(Repository orginalRepository, TextFolder folder)
        {
            _textFolderManager.Remove(orginalRepository, folder);

            Sync(Site.Current, orginalRepository, (Repository targetRepository) =>
            {
                TextFolder targetFolder = _textFolderManager.Get(targetRepository, folder.FullName);
                if (targetFolder != null)
                {
                    EnsureFolder(targetRepository, folder);
                    _textFolderManager.Remove(targetRepository, folder);
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

        private void EnsureFolder(Repository orginalRepository, TextFolder folder)
        {
            Folder parentFolder = folder.Parent;
            while (parentFolder != null)
            {
                parentFolder.Repository = orginalRepository;
                parentFolder = parentFolder.Parent;
            }
        }
    }
}