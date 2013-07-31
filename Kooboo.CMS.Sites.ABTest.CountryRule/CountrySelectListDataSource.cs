using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kooboo.CMS.Sites.ABTest.CountryRule
{
    public class CountrySelectListDataSource : ISelectListDataSource
    {
        #region ISelectListDataSource Members

        public IEnumerable<SelectListItem> GetSelectListItems(RequestContext requestContext, string filter)
        {
            var regions = GetCountryList();
            foreach (var r in regions.OrderBy(c => c.DisplayName))
            {
                yield return new SelectListItem() { Text = r.EnglishName, Value = r.Name, Selected = r.Equals(new RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.LCID)) };
            }
        }

        #endregion

        public static IEnumerable<RegionInfo> GetCountryList()
        {
            //create a new Generic list to hold the country names returned
            Dictionary<string, RegionInfo> regions = new Dictionary<string, RegionInfo>();

            //create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
            //cultures installed with the .Net Framework
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            //loop through all the cultures found
            foreach (CultureInfo culture in cultures)
            {
                //pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
                //to the RegionInfo contructor to gain access to the information for that culture
                RegionInfo region = new RegionInfo(culture.LCID);

                //make sure out generic list doesnt already
                //contain this country
                if (!(regions.ContainsKey(region.EnglishName)))
                    //not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
                    //value to our generic list
                    regions.Add(region.EnglishName, region);
            }
            return regions.Values;
        }


    }
}
