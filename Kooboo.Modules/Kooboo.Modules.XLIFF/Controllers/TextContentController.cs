using Kooboo.CMS.Common;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Query.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kooboo.CMS.Content.Query;
using Kooboo.Web;
using Kooboo.CMS.Web.Areas.Contents.Controllers;
using Kooboo.Modules.XLIFF.Models;

namespace Kooboo.Modules.XLIFF.Controllers
{
    public class TextContentController : ContentControllerBase
    {
        #region Import
        public virtual ActionResult TextContentImport(TextContentImportModel model)
        {
            ModelState.Clear();
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult TextContentImport(TextContentImportModel model, string @return)
        {
            var data = new JsonResultData(ModelState);
            data.RunWithTry((resultData) =>
            {
                model.TextContentExporter.Import(new TextFolder(Repository, model.FolderName), model.File.InputStream);
                data.RedirectUrl = @return;
            });
            return Json(data);
        }
        #endregion

        #region Export
        public virtual void Export(string Formatter, string folderName, string[] docs)
        {
            var exporter = Kooboo.CMS.Common.Runtime.EngineContext.Current.Resolve<ITextContentFormater>((Formatter??""));
            var fileName = folderName + exporter.FileExtension;
            Response.AttachmentHeader(fileName);

            var textFolder = new TextFolder(Repository, folderName);

            var contentQuery = textFolder.CreateQuery();
            foreach (var item in docs)
            {
                contentQuery = contentQuery.Or(new WhereEqualsExpression(null, "UUID", item));
            }
            exporter.Export(contentQuery, Response.OutputStream);
        }
        #endregion
    }
}
