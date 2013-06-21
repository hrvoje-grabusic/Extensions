using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.CMS.Sites.ABTest.CountryRule
{
    public interface IIPToCountryService
    {
        string QueryCountry(string ip);
    }
}
