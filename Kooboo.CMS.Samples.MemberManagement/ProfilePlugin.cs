using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using Kooboo.CMS.Content.Query;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
namespace Kooboo.CMS.Samples.MemberManagement
{
    public class ProfilePlugin : IHttpMethodPagePlugin
    {


        public System.Web.Mvc.ActionResult HttpGet(Page_Context context, PagePositionContext positionContext)
        {
            return null;
        }

        public System.Web.Mvc.ActionResult HttpPost(Page_Context context, PagePositionContext positionContext)
        {
            AntiForgery.Validate();

            try
            {
                var httpContext = context.ControllerContext.HttpContext;
                var repository = Repository.Current;
                var textFolder = new TextFolder(repository, "Members");
                var userContent = MemberAuth.GetMemberContent();

                var email = httpContext.Request.Form["Email"];
                var language = httpContext.Request.Form["Language"];

                ServiceFactory.TextContentManager.Update(textFolder, userContent.UUID, new string[] { "Email", "Language" }, new object[] { email, language });
            }
            catch (Exception e)
            {
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", e);
                Kooboo.HealthMonitoring.Log.LogException(e);
            }
            return null;
        }
    }
}