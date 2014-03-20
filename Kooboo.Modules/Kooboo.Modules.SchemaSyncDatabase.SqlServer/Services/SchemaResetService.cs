using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Models.Paths;
using Kooboo.CMS.Content.Persistence;
using Kooboo.Modules.SchemaSyncDatabase.SqlServer.SqlServer.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Modules.SchemaSyncDatabase.SqlServer.Services
{
    [Dependency(typeof(ISchemaResetService), Key = "SchemaResetService")]
    public class SchemaResetService : ISchemaResetService
    {
        private ISchemaProvider _schemaProvider { get; set; }

        private IGetSchemaProvider _getSchemaProvider { get; set; }
        public SchemaResetService(ISchemaProvider schemaProvider, IGetSchemaProvider getSchemaProvider)
        {
            this._schemaProvider = schemaProvider;
            this._getSchemaProvider = getSchemaProvider;
        }

        public int ResetSchema(Repository repository)
        {
            var root = SchemaPath.GetBaseDir(repository);

            var paths = Directory.GetDirectories(root);
            var schemas = Kooboo.CMS.Content.Services.ServiceFactory.SchemaManager.All(repository, "").ToArray();

            return ResetSchema(repository, schemas, paths);
        }

        public int ResetSchema(Repository repository, Schema[] schemas)
        {
            var root = SchemaPath.GetBaseDir(repository);
            var paths = schemas.Select(it => Path.Combine(root, it.Name));

            return ResetSchema(repository, schemas, paths);
        }

        protected int ResetSchema(Repository repository, Schema[] schemas, IEnumerable<string> paths)
        {
            foreach (var item in schemas)
            {
                item.Repository = repository;
                var dummy = _getSchemaProvider.GetSchema(item);
                var current = item.AsActual();
                Kooboo.CMS.Content.Services.ServiceFactory.SchemaManager.Update(repository, current, dummy);
            }

            var stream = new MemoryStream();
            Kooboo.CMS.Content.Persistence.Default.ImportHelper.Export(paths, stream);
            stream.Position = 0;

            _schemaProvider.Import(repository, stream, true);
            return paths.Count();
        }
    }



    public interface ISchemaResetService
    {
        /// <summary>
        /// Reset all schemas in indicated repository
        /// </summary>
        /// <param name="repository">repository entity</param>
        /// <returns>counts of schemas effected</returns>
        int ResetSchema(Repository repository);

        /// <summary>
        /// Reset indicated schemas in repository
        /// </summary>
        /// <param name="repository">repository entity</param>
        /// <param name="schemas">schema array</param>
        /// <returns>counts of schemas effected</returns>
        int ResetSchema(Repository repository, Schema[] schemas);

    }
}
