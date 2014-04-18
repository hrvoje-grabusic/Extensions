using Kooboo.CMS.Sites.Extension.UI;
using Kooboo.CMS.Sites.Extension.UI.TopToolbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kooboo.Globalization;

namespace Kooboo.Modules.XLIFF.UI
{
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(IToolbarProvider))]
    public class ImportExportToolbarsProvider : IToolbarProvider
    {
        private const string GroupName = "XLIFF";
        private const string AreaName = "XLIFF";

        public MvcRoute[] ApplyTo
        {
            get
            {
                return new[]{
                    new MvcRoute(){
                        Area="Contents",
                        Controller="TextContent",
                        Action="Index"
                    }
                };
            }
        }

        public IEnumerable<ToolbarButton> GetButtons(System.Web.Routing.RequestContext requestContext)
        {
            return new ToolbarButton[]{
                 new ToolbarButton(){
                    GroupName = GroupName,
                    CommandTarget = new MvcRoute(){ Action="TextContentImport",Controller="TextContent",Area=AreaName,RouteValues=new Dictionary<string,object>(){
                        {"Formatter",GroupName}}
                    },
                    CommandText="Import".Localize()
                },
                //new ToolbarButton(){
                //    GroupName = GroupName,
                //    CommandTarget = new MvcRoute(){ Action="Export",Controller="TextContent",Area=AreaName,RouteValues=new Dictionary<string,object>(){
                //        {"Formatter",GroupName}}
                //    },
                //    CommandText="Export current folder".Localize(),
                //    HtmlAttributes = new Dictionary<string,object>(){
                //        {"data-command-type","Download"}
                //    }
                //},
                new ToolbarButton(){
                    GroupName = GroupName,
                    CommandTarget = new MvcRoute(){ Action="Export",Controller="TextContent",Area=AreaName,RouteValues=new Dictionary<string,object>(){
                        {"Formatter",GroupName}}
                    },
                    CommandText="Export".Localize(),
                    HtmlAttributes = new Dictionary<string,object>(){
                        {"data-show-on-check","Any"},
                        {"data-command-type","Download"}
                    }
                }
           };
        }

        public IEnumerable<ToolbarGroup> GetGroups(System.Web.Routing.RequestContext requestContext)
        {
            return new ToolbarGroup[]{ new ToolbarGroup()
            {
                GroupName = GroupName,
                DisplayText = "XLIFF".Localize(),
                IconClass="export"
            }};
        }
    }
}
