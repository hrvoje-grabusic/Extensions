using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Sites.Globalization;
using Kooboo.Globalization;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
namespace Kooboo.CMS.Samples.MemberManagement
{
    public class ResetPasswordPlugin : IHttpMethodPagePlugin
    {

        public virtual bool ValidateMemberPasswordToken(string userName, string token)
        {
            var repository = Repository.Current;
            var textFolder = new TextFolder(repository, "Members");
            var member = textFolder.CreateQuery().WhereEquals("UserName", userName).FirstOrDefault();

            if (member == null)
            {
                return false;
            }
            string dbToken = member.Get<string>("ForgotPWToken");
            var passwordToken = dbToken == null ? "" : dbToken.ToString();
            if (string.IsNullOrEmpty(passwordToken) || !passwordToken.EqualsOrNullEmpty(token, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public System.Web.Mvc.ActionResult HttpGet(Page_Context context, PagePositionContext positionContext)
        {
            HttpRequestBase request = context.ControllerContext.HttpContext.Request;
            Controller controller = (Controller)context.ControllerContext.Controller;
            string username = request.Params["UserName"];
            string token = request.Params["token"];
            if (!ValidateMemberPasswordToken(username, token))
            {
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", "The password token is invalid.".Localize());
            }
            return null;
        }

        public System.Web.Mvc.ActionResult HttpPost(Page_Context context, PagePositionContext positionContext)
        {
            HttpRequestBase request = context.ControllerContext.HttpContext.Request;
            Controller controller = (Controller)context.ControllerContext.Controller;
            string username = request.Params["UserName"];
            string token = request.Params["token"];
            if (!ValidateMemberPasswordToken(username, token))
            {
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", "The password token is invalid.".Localize());
                return null;
            }
            AntiForgery.Validate();

            var newPassword = request.Form["newpassword"];
            var confirmPassword = request.Form["confirmPassword"];
            if (newPassword != confirmPassword)
            {
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", "The passwords do not match.".RawLabel().ToString());
                return null;
            }
            try
            {
                var httpContext = context.ControllerContext.HttpContext;
                var repository = Repository.Current;
                var textFolder = new TextFolder(repository, "Members");
                var content = textFolder.CreateQuery().WhereEquals("UserName", username).FirstOrDefault();

                var passwordSalt = "";
                if (content["PasswordSalt"] == null)
                {
                    passwordSalt = MemberAuth.GenerateSalt();
                }
                else
                {
                    passwordSalt = content["PasswordSalt"].ToString();
                }

                newPassword = MemberAuth.EncryptPassword(newPassword, passwordSalt);

                ServiceFactory.TextContentManager.Update(textFolder, content.UUID,
                    new string[] { "Password", "ForgotPWToken", "PasswordSalt" }, new object[] { newPassword, "", passwordSalt });
                context.ControllerContext.Controller.ViewBag.Message = "The password has been changed.".Label();

                MemberAuth.SetAuthCookie(username, false);
                return new RedirectResult(context.Url.FrontUrl().PageUrl("Dashboard").ToString());
            }
            catch (Exception e)
            {
                context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", e.Message);
                Kooboo.HealthMonitoring.Log.LogException(e);
            }
            return null;
        }
    }
}