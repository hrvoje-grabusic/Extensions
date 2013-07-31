using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.CMS.Sites.ABTest.CountryRule
{
    public class MaxMindService : IIPToCountryService
    {
        LookupService _lookupService;
        public MaxMindService()
            : this("~/App_Data/GeoIP.dat")
        {
        }
        public MaxMindService(string dataFile)
        {
            string path = Kooboo.Web.Url.UrlUtility.MapPath("~/App_Data/GeoIP.dat");
            _lookupService = new LookupService(path, LookupService.GEOIP_MEMORY_CACHE);
        }
        public string QueryCountry(string ip)
        {       
            //get country of the ip address
            Country c = _lookupService.getCountry(ip);
            return c.getCode();
        }
    }
}
