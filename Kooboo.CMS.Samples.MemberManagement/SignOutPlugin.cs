using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Sites.Globalization;
using System.Web.Mvc;

namespace Kooboo.CMS.Samples.MemberManagement
{
    public class SignOutPlugin : IHttpMethodPagePlugin
    {
        public ActionResult HttpGet(Page_Context context, PagePositionContext positionContext)
        {
            return SignOut(context);
        }

        public ActionResult HttpPost(Page_Context context, PagePositionContext positionContext)
        {
            return SignOut(context);
        }

        private ActionResult SignOut(Page_Context context)
        {
            MemberAuth.SignOut();
            return new RedirectResult(context.Url.FrontUrl().PageUrl("SignIn").ToString());
        }
    }
}