using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Sites.Globalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
namespace Kooboo.CMS.Samples.MemberManagement
{
    public class ChangePasswordPlugin : IHttpMethodPagePlugin
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

                var oldPassword = httpContext.Request.Form["OldPassword"];
                var newPassword = httpContext.Request.Form["NewPassword"];

                if (userContent["PasswordSalt"] != null)
                {
                    oldPassword = MemberAuth.EncryptPassword(oldPassword, userContent["PasswordSalt"].ToString());
                }
                if (userContent["password"].ToString() == oldPassword)
                {
                    var passwordSalt = "";
                    if (userContent["PasswordSalt"] == null)
                    {
                        passwordSalt = MemberAuth.GenerateSalt();
                    }
                    else
                    {
                        passwordSalt = userContent["PasswordSalt"].ToString();
                    }

                    newPassword = MemberAuth.EncryptPassword(newPassword, passwordSalt);

                    ServiceFactory.TextContentManager.Update(textFolder, userContent.UUID, new string[] { "Password", "PasswordSalt" }, new object[] { newPassword, passwordSalt });
                    context.ControllerContext.Controller.ViewBag.Message = "The password has been changed.".RawLabel().ToString();
                }
                else
                {
                    context.ControllerContext.Controller.ViewData.ModelState.AddModelError("", "The old password is invalid.".RawLabel().ToString());
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