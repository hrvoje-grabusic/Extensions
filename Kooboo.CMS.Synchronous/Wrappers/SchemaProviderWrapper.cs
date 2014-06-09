using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Sites.Persistence;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Sites.Models;

namespace Kooboo.CMS.Synchronous.SyncContentSchema
{
    public class SchemaProviderWrapper : ISchemaProvider
    {
        private ISchemaProvider _schemaProvider;
        private ISiteProvider _siteProvider;

        public SchemaProviderWrapper(ISchemaProvider schemaProvider, ISiteProvider siteProvider)
        {
            _schemaProvider = schemaProvider;
            _siteProvider = siteProvider;
        }

        #region ISchemaProvider Members

        public Schema Copy(Repository orginalRepository, string sourceName, string destName)
        {
            Schema schema = _schemaProvider.Copy(orginalRepository, sourceName, destName);

            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                _schemaProvider.Copy(targetRepository, sourceName, destName);
            });

            return schema;
        }

        public Schema Create(Repository orginalRepository, string schemaName, Stream stream)
        {
            Schema schema = _schemaProvider.Create(orginalRepository, schemaName, stream);

            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                _schemaProvider.Create(targetRepository, schemaName, stream);
            });

            return schema;
        }

        public void Export(Repository orginalRepository, IEnumerable<Schema> schemas, Stream stream)
        {
            _schemaProvider.Export(orginalRepository, schemas, stream);
        }

        public void Import(Repository orginalRepository, Stream stream, bool @override)
        {
            _schemaProvider.Import(orginalRepository, stream, @override);

            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                _schemaProvider.Import(targetRepository, stream, @override);
            });
        }

        public void Initialize(Schema schema)
        {
            _schemaProvider.Initialize(schema);
        }

        #endregion

        #region IContentElementProvider<T> Members

        IEnumerable<Schema> IContentElementProvider<Schema>.All(Repository repository)
        {
            return _schemaProvider.All(repository);
        }

        #endregion

        #region IProvider<T> Members

        public Schema Get(Schema schema)
        {
            return _schemaProvider.Get(schema);
        }

        public void Add(Schema schema)
        {
            _schemaProvider.Add(schema);

            Repository orginalRepository = schema.Repository;
            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                schema.Repository = targetRepository;
                _schemaProvider.Add(schema);
            });

            schema.Repository = orginalRepository;
        }

        public void Remove(Schema schema)
        {
            _schemaProvider.Remove(schema);

            Repository orginalRepository = schema.Repository;
            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                schema.Repository = targetRepository;
                _schemaProvider.Remove(schema);
            });

            schema.Repository = orginalRepository;
        }

        public void Update(Schema newSchema, Schema oldSchema)
        {
            _schemaProvider.Update(newSchema, oldSchema);

            Repository orginalRepository = newSchema.Repository;
            Sync(Site.Current, orginalRepository, delegate(Repository targetRepository)
            {
                newSchema.Repository = targetRepository;

                Schema schema = _schemaProvider.Get(newSchema);
                if (schema != null)
                {
                    _schemaProvider.Update(newSchema, schema);
                }
            });

            newSchema.Repository = orginalRepository;
        }

        public IEnumerable<Schema> All()
        {
            return _schemaProvider.All();
        }

        #endregion

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