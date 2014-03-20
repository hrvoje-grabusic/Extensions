using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Persistence.SqlServer;
using Kooboo.Modules.SchemaSyncDatabase.SqlServer.SqlServer.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Kooboo.Modules.SchemaSyncDatabase.SqlServer.SqlServer.DatabaseProvider
{
    [Dependency(typeof(IGetSchemaProvider), Key = "GetSchemaProvider")]
    public class GetSchemaProviderSqlServer : IGetSchemaProvider
    {
        private const string sqlGetColumnsFormat = "SELECT [name] FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [name] = '{0}')";
        public Schema GetSchema(Schema schema)
        {
            var systemFields = schema.Columns.Where(it => it.IsSystemField).Select(it => it.Name);
            schema.Columns.Clear();
            var tableName = GetTableName(schema);
            var sql = String.Format(sqlGetColumnsFormat, tableName);
            SqlCommand command = new SqlCommand() { CommandText = sql };
            var connection = new SqlConnection();
            var fields = SQLServerHelper.ExecuteReader(schema.Repository, command, out connection) as SqlDataReader;
            while (fields.Read())
            {
                var fieldName = fields[0].ToString();
                if (!systemFields.Contains(fieldName, new equal()))
                {
                    var column = new Column();
                    column.Name = fieldName;
                    schema.Columns.Add(column);
                }
            }
            schema.Columns.RemoveAll(it => it.IsSystemField);
            return schema;
        }


        public string GetTableName(Schema schema)
        {
            return String.Format("{0}.{1}", schema.Repository.Name, schema.Name);
        }
    }
    class equal : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
