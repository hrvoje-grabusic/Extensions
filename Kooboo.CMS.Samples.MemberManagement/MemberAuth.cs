using Kooboo.CMS.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Kooboo.CMS.Sites.View;
using Kooboo.CMS.Content.Query;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.CMS.Samples.MemberManagement
{
    public class MemberAuth
    {
        #region Form auth
        public static string AuthCookieName = ".MemberAUTH";
        public static string MemberPrincipalName = "MemberPrincipal";

        //public static void Authenticate(System.Web.Mvc.UrlHelper urlHelper)
        //{
        //    var customer = CommerceAuthentication.GetCustomer();
        //    if (customer == null || customer.Identity.IsAuthenticated == false)
        //    {
        //        urlHelper.RequestContext.HttpContext.Response.Redirect(urlHelper.FrontUrl().PageUrl("Member/Login", new { returnUrl = urlHelper.RequestContext.HttpContext.Request.RawUrl }).ToString());
        //    }
        //}
        public static TextContent GetMemberContent()
        {
            return ContentHelper.TextFolder("Members").CreateQuery().WhereEquals("UserName", GetMember().Identity.Name).FirstOrDefault();
        }
        public static bool IsAuthenticated()
        {
            var principal = MemberAuth.GetMember();
            if (principal != null && principal.Identity.IsAuthenticated == true)
            {
                return true;
            }
            return false;
        }
        public static void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
            cookie.Name = AuthCookieName;

            HttpContext.Current.Response.SetCookie(cookie);
        }
        public static void SignOut()
        {
            var authCookie = GetAuthCookie(HttpContext.Current.Request.Cookies);
            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddDays(-100);
                HttpContext.Current.Response.SetCookie(authCookie);
            }
        }
        public static IPrincipal GetMember()
        {
            IPrincipal memberPrincipal = (IPrincipal)HttpContext.Current.Items[MemberPrincipalName];
            if (memberPrincipal == null)
            {
                memberPrincipal = DefaultPrincipal();

                var authCookie = GetAuthCookie(HttpContext.Current.Request.Cookies);
                if (authCookie != null && authCookie.Expires < DateTime.Now)
                {
                    var encryptedTicket = authCookie.Value;
                    var ticket = FormsAuthentication.Decrypt(encryptedTicket);
                    if (!ticket.Expired)
                    {
                        memberPrincipal = new GenericPrincipal(new FormsIdentity(ticket), new string[0]);
                    }
                }

                HttpContext.Current.Items[MemberPrincipalName] = memberPrincipal;
            }
            return memberPrincipal;
        }

        private static GenericPrincipal DefaultPrincipal()
        {
            return new GenericPrincipal(new GenericIdentity(""), new string[0]);
        }
        private static HttpCookie GetAuthCookie(HttpCookieCollection cookies)
        {
            for (int i = 0; i < cookies.Count; i++)
            {
                var cookie = cookies[i];
                if (cookie.Name == AuthCookieName)
                {
                    return cookie;
                }
            }
            return null;
        } 
        #endregion
        
        #region Password
        public static string GenerateSalt()
        {
            byte[] data = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public static string EncryptPassword(string pass, string salt)
        {

            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] inArray = null;


            KeyedHashAlgorithm algorithm = KeyedHashAlgorithm.Create();
            algorithm.Key = new byte[64]; //compatible with mono
            if (algorithm.Key.Length == src.Length)
            {
                algorithm.Key = src;
            }
            else if (algorithm.Key.Length < src.Length)
            {
                byte[] dst = new byte[algorithm.Key.Length];
                Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                algorithm.Key = dst;
            }
            else
            {
                int num2;
                byte[] buffer5 = new byte[algorithm.Key.Length];
                for (int i = 0; i < buffer5.Length; i += num2)
                {
                    num2 = Math.Min(src.Length, buffer5.Length - i);
                    Buffer.BlockCopy(src, 0, buffer5, i, num2);
                }
                algorithm.Key = buffer5;
            }
            inArray = algorithm.ComputeHash(bytes);

            return Convert.ToBase64String(inArray);
        }
        #endregion
    }
}