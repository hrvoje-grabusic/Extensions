using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Web.UI;

[assembly: WebResource("Kooboo.CMS.Toolkit.Controls.Resources.Kooboo.CMS.Toolkit.Controls.js", "text/javascript")]
[assembly: WebResource("Kooboo.CMS.Toolkit.Controls.Resources.jquery.jCombo.min.js", "text/javascript")]
[assembly: WebResource("Kooboo.CMS.Toolkit.Controls.Resources.DateTimePicker.css", "text/css")]
namespace Kooboo.CMS.Toolkit.Controls
{
    public class ControlsScript
    {
        public static string GetWebResourceUrl()
        {
            var page = new Page();
            var type = typeof(ControlsScript);
            page.ClientScript.RegisterClientScriptResource(type, "Kooboo.CMS.Toolkit.Controls.Resources.Kooboo.CMS.Toolkit.Controls.js");
            return page.ClientScript.GetWebResourceUrl(type, "Kooboo.CMS.Toolkit.Controls.Resources.Kooboo.CMS.Toolkit.Controls.js");
        }

        public static string GetJComboResourceUrl()
        {
            var page = new Page();
            var type = typeof(ControlsScript);
            page.ClientScript.RegisterClientScriptResource(type, "Kooboo.CMS.Toolkit.Controls.Resources.jquery.jCombo.min.js");
            return page.ClientScript.GetWebResourceUrl(type, "Kooboo.CMS.Toolkit.Controls.Resources.jquery.jCombo.min.js");
        }

        public static string GetDatetimeResourceUrl()
        {
            var page = new Page();
            var type = typeof(ControlsScript);
            page.ClientScript.RegisterClientScriptResource(type, "Kooboo.CMS.Toolkit.Controls.Resources.DateTimePicker.css");
            return page.ClientScript.GetWebResourceUrl(type, "Kooboo.CMS.Toolkit.Controls.Resources.DateTimePicker.css");
        }
    }
}