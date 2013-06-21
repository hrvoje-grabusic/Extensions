#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.CMS.Sites.ABTest;
using System.Runtime.Serialization;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Kooboo.Web.Mvc;
namespace Kooboo.CMS.Sites.ABTest.CountryRule
{
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(IVisitRule), Kooboo.CMS.Common.Runtime.Dependency.ComponentLifeStyle.Transient, Key = "CountryVisitRule")]   
    [System.Runtime.Serialization.DataContract(Name = "CountryVisitRule")]
    [System.Runtime.Serialization.KnownType(typeof(CountryVisitRule))]
    public class CountryVisitRule : CustomRuleBase, IVisitRule
    {
        static IIPToCountryService _ipToCountry = new MaxMindService();

        [Required(ErrorMessage = "Required")]
        [DataMember]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Country")]
        [UIHint("Dropdownlist")]
        [DataSource(typeof(CountrySelectListDataSource))]
        [DataMember]
        public string CountryCode { get; set; }

        public bool IsMatch(System.Web.HttpRequestBase httpRequest)
        {
            var ip = httpRequest.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = httpRequest.UserHostAddress;
            }
            var isMatched = false;
            if (!IPAddress.IsLoopback(IPAddress.Parse(ip)))
            {
                var country = _ipToCountry.QueryCountry(ip);
                if (!string.IsNullOrEmpty(country))
                {
                    isMatched = country.EqualsOrNullEmpty(CountryCode, StringComparison.OrdinalIgnoreCase);
                }
            }
            return isMatched;
        }


        public override string RuleType
        {
            get { return "Country"; }
            set { }
        }

        public override string DisplayText
        {
            get { return "return this.CountryCode();"; }
            set { }
        }
    }
}
