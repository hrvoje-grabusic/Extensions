using Kooboo.CMS.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Modules.SchemaSyncDatabase.SqlServer.SqlServer.Services
{
    public interface IGetSchemaProvider
    {
        Schema GetSchema(Schema schema);

        string GetTableName(Schema schema);
    }


}
