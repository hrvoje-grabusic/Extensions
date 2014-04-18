using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using Kooboo.Web.Mvc;
using Kooboo.CMS.Common;
using Kooboo.CMS.Account.Services;
using Kooboo.CMS.Account.Models;

namespace Kooboo.Modules.XLIFF
{
    public class AreaRegistration : AreaRegistrationEx
    {
        public override string AreaName
        {
            get
            {
                return "XLIFF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}",
                new { action = "Index" }
                , null
                , new[] { "Kooboo.Modules.XLIFF.Controllers", "Kooboo.Web.Mvc", "Kooboo.Web.Mvc.WebResourceLoader" }
            );

            //Kooboo.Web.Mvc.Menu.MenuFactory.RegisterAreaMenu(AreaName, AreaHelpers.CombineAreaFilePhysicalPath(AreaName, "Menu.config"));
            Kooboo.Web.Mvc.WebResourceLoader.ConfigurationManager.RegisterSection(AreaName, AreaHelpers.CombineAreaFilePhysicalPath("Sites", "WebResources.config"));

            #region RegisterPermissions
            var roleManager = Kooboo.CMS.Common.Runtime.EngineContext.Current.Resolve<RoleManager>();
            roleManager.AddPermission(Permission.Contents_SettingPermission);
            roleManager.AddPermission(Permission.Contents_SchemaPermission);
            roleManager.AddPermission(Permission.Contents_FolderPermission);
            roleManager.AddPermission(Permission.Contents_ContentPermission);
            roleManager.AddPermission(Permission.Contents_BroadcastingPermission);
            roleManager.AddPermission(Permission.Contents_WorkflowPermission);
            roleManager.AddPermission(Permission.Contents_SearchSettingPermission);
            roleManager.AddPermission(Permission.Contents_HtmlBlockPermission);

            #endregion

            base.RegisterArea(context);
        }
    }
}
