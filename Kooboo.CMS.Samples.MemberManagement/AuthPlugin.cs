using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.CMS.Samples.MemberManagement
{
    public class AuthPlugin : IHttpMethodPagePlugin
    {
        public System.Web.Mvc.ActionResult HttpGet(CMS.Sites.View.Page_Context context, CMS.Sites.View.PagePositionContext positionContext)
        {
            return Authorizate(context);
        }

        public System.Web.Mvc.ActionResult HttpPost(CMS.Sites.View.Page_Context context, CMS.Sites.View.PagePositionContext positionContext)
        {
            return Authorizate(context);
        }
        private ActionResult Authorizate(Page_Context context)
        {
            if (!MemberAuth.IsAuthenticated())
            {
                return new RedirectResult(context.Url.FrontUrl().PageUrl("SignIN", new { returnUrl = context.ControllerContext.HttpContext.Request.RawUrl }).ToString());
            }
            return null;
        }
    }
}