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
    public class SignInPlugin : IHttpMethodPagePlugin
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

                string username = httpContext.Request.Form["username"];
                string password = httpContext.Request.Form["password"];


                var member = textFolder.CreateQuery().WhereEquals("UserName", username).FirstOrDefault();

                if (member != null)
                {                   
                    var encryptedPassword = password;
                    if (member["PasswordSalt"] != null)
                    {
                        var passwordSalt = member["PasswordSalt"].ToString();
                        encryptedPassword = MemberAuth.EncryptPassword(password, passwordSalt);
                    }                  
                    if (encryptedPassword == member["Password"].ToString())
                    {
                        var rememberme = httpContext.Request.Form["rememberMe"].Contains("true");
                        var returnUrl = httpContext.Request.QueryString["returnUrl"];
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            returnUrl = context.Url.FrontUrl().PageUrl("Dashboard").ToString();
                        }
                        MemberAuth.SetAuthCookie(username, rememberme);
                        return new RedirectResult(returnUrl);
                    }
                }
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", "Username or password is invalid".RawLabel().ToString());
                return null;

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