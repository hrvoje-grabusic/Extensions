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
using System.Net.Mail;
using Kooboo.CMS.Sites.Models;

namespace Kooboo.CMS.Samples.MemberManagement
{
    public class ForgotPasswordPlugin : IHttpMethodPagePlugin
    {

        public System.Web.Mvc.ActionResult HttpGet(Page_Context context, PagePositionContext positionContext)
        {
            return null;
        }

        /// <summary>
        /// 暂时用163邮箱发送邮件，发件人display="info@tenutadiseripa.com"。可支持异步发送。
        /// </summary>
        /// <param name="toEmail">收件人</param>
        /// <param name="subject">邮件subject</param>
        /// <param name="body">邮件body</param>
        /// <param name="isAsync">是否异步发送</param>
        /// <param name="completedMethod">若是异步发送邮件，则可在此定义异步发送完成之后要执行的事件</param>
        public void SendMail(string toEmail, string subject, string body,
            bool isAsync = false, SendCompletedEventHandler completedMethod = null)
        {
            var smtp = Site.Current.Smtp;
            if (smtp == null)
            {
                throw new KoobooException("Smtp setting is null".Localize());
            }


            MailMessage mailMessage = new MailMessage() { From = new MailAddress(smtp.From) };

            mailMessage.To.Add(new MailAddress(toEmail, ""));

            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;

            SmtpClient smtpClient = smtp.ToSmtpClient();

            if (!isAsync)
            {
                smtpClient.Send(mailMessage);
            }
            else
            {//异步发送
                if (completedMethod != null)
                {
                    smtpClient.SendCompleted += new SendCompletedEventHandler(completedMethod);//注册异步发送邮件完成时的事件  
                }
                smtpClient.SendAsync(mailMessage, mailMessage.Body);
            }
        }

        public System.Web.Mvc.ActionResult HttpPost(Page_Context context, PagePositionContext positionContext)
        {
            HttpRequestBase request = context.ControllerContext.HttpContext.Request;
            Controller controller = (Controller)context.ControllerContext.Controller;
            string username = request.Form["username"];
            string email = request.Form["email"];
            try
            {
                if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                {
                    controller.ViewData.ModelState.AddModelError("", "Username or Email is required.".Localize());
                    return null;
                }
                else if (controller.ViewData.ModelState.IsValid)
                {
                    var repository = Repository.Current;
                    var textFolder = new TextFolder(repository, "Members");
                    TextContent content = null;
                    if (!string.IsNullOrEmpty(username))
                    {
                        content = textFolder.CreateQuery().WhereEquals("UserName", username).FirstOrDefault();
                        email = content.Get<string>("Email");
                    }
                    else
                    {
                        content = textFolder.CreateQuery().WhereEquals("Email", email).FirstOrDefault();
                        username = content.Get<string>("UserName");
                    }
                    if (content != null)
                    {
                        string randomValue = Kooboo.UniqueIdGenerator.GetInstance().GetBase32UniqueId(16);
                        ServiceFactory.TextContentManager.Update(textFolder, content.UUID, new string[] { "ForgotPWToken" }, new object[] { randomValue });

                        string link = new Uri(request.Url, string.Format("ResetPassword?UserName={0}&token={1}".RawLabel().ToString(), username, randomValue)).ToString();
                        string emailBody = "<b>{0}</b> <br/><br/> To change your password, click on the following link:<br/> <br/> <a href='{1}'>{1}</a> <br/>".RawLabel().ToString();
                        string subject = "Reset your password".RawLabel().ToString();
                        string body = string.Format(emailBody, username, link);
                        SendMail(email, subject, body, false);
                    }
                    else
                    {
                        controller.ViewData.ModelState.AddModelError("", "The user does not exists.".RawLabel().ToString());
                    }
                    controller.ViewBag.Message = "An email with instructions to choose a new password has been sent to you.".RawLabel().ToString();
                }
            }
            catch (Exception e)
            {
                controller.ViewData.ModelState.AddModelError("", e.Message);
            }

            return null;
        }
    }
}
