using Kooboo.CMS.Common;
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Persistence;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites;
using Kooboo.CMS.Web.Models;
using Kooboo.Modules.SchemaSyncDatabase.SqlServer.Services;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kooboo.Globalization;

namespace Kooboo.Modules.SchemaSyncDatabase.SqlServer.Controllers
{
    public class SchemasController : Kooboo.CMS.Web.Areas.Contents.Controllers.ManagerControllerBase
    {
        private ISchemaResetService _service { get; set; }
        public SchemasController(ISchemaResetService service)
        {
            this._service = service;
        }

        [HttpPost]
        public virtual ActionResult ResetAll(string repositoryName)
        {
            var data = new JsonResultData(ModelState);
            if (ModelState.IsValid)
            {
                data.RunWithTry((resultData) =>
                {
                    var repository = Kooboo.CMS.Content.Services.ServiceFactory.RepositoryManager.Get(repositoryName);
                    var count = _service.ResetSchema(repository);
                    data.AddMessage(String.Format("Reset {0} Schemas", count));
                });
            }
            return Json(data);
        }

        [HttpPost]
        public virtual ActionResult ResetSpecified(Schema[] model, string repositoryName)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                if (model != null)
                {
                    var repository = Kooboo.CMS.Content.Services.ServiceFactory.RepositoryManager.Get(repositoryName);
                    var count = _service.ResetSchema(repository, model);
                    data.AddMessage(String.Format("Reset {0} Schemas", count));
                }
            });

            return Json(data);
        }

    }
}
