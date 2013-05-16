using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Sites.Globalization;
namespace Kooboo.CMS.Samples.MemberManagement
{
    public class RegisterPlugin : IHttpMethodPagePlugin
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
                var values = new NameValueCollection(httpContext.Request.Form);
                values["Published"] = true.ToString();

                var member = textFolder.CreateQuery().WhereEquals("UserName", values["username"]).FirstOrDefault();
                if (member != null)
                {
                    context.ControllerContext.Controller.ViewData.ModelState.AddModelError("UserName", "The user already exists.".RawLabel().ToString());                 
                }
                else
                {
                    values["PasswordSalt"] = MemberAuth.GenerateSalt();
                    values["Password"] = MemberAuth.EncryptPassword(values["Password"], values["PasswordSalt"]);

                    var textContext = ServiceFactory.TextContentManager.Add(repository, textFolder, null, null,
                       values, httpContext.Request.Files, null, httpContext.User.Identity.Name);

                    MemberAuth.SetAuthCookie(textContext["UserName"].ToString(), false);

                    return new RedirectResult(context.Url.FrontUrl().PageUrl("Dashboard").ToString());
                }

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