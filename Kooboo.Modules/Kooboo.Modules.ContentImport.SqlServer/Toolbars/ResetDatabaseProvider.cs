using Kooboo.CMS.Sites.Extension.UI;
using Kooboo.CMS.Sites.Extension.UI.TopToolbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Kooboo.Globalization;

namespace Kooboo.Modules.SchemaSyncDatabase.SqlServer
{
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(IToolbarProvider), Key = "ResetDatabaseToolbarProvider")]
    public class ResetDatabaseProvider : IToolbarProvider
    {
        public MvcRoute[] ApplyTo
        {
            get
            {
                return new[]{
                    new MvcRoute(){
                        Area="Contents",
                        Controller="ContentType",
                        Action="Index"
                    }
                };
            }
        }

        public IEnumerable<ToolbarButton> GetButtons(RequestContext requestContext)
        {
            return new ToolbarButton[]{
                new ToolbarButton(){
                    GroupName="Database",
                    CommandTarget = new MvcRoute(){ Action="ResetAll",Controller="Schemas",Area="ResetDatabase"},
                    CommandText="All items".Localize(),
                    HtmlAttributes = new Dictionary<string,object>(){
                        {"data-ajax","post"},
                        {"data-confirm","Are you sure want to reset all schemas to Database?"}
                    }
                },
                new ToolbarButton(){
                    GroupName="Database",
                    CommandTarget = new MvcRoute(){ Action="ResetSpecified",Controller="Schemas",Area="ResetDatabase"},
                    CommandText="Specified items".Localize(),
                    HtmlAttributes = new Dictionary<string,object>(){
                        {"data-show-on-check","Any"},
                        {"data-command-type","AjaxPost"},
                        {"data-confirm","Are you sure want to reset specified schemas to Database?"}
                    }
                }
           };
        }

        public IEnumerable<ToolbarGroup> GetGroups(RequestContext requestContext)
        {
            return new ToolbarGroup[]{ new ToolbarGroup()
            {
                GroupName = "Database",
                DisplayText = "Update Database".Localize(),
                IconClass="save"
            }};
        }
    }
}
