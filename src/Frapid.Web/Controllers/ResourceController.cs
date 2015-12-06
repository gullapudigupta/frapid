﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Frapid.Areas;
using Frapid.i18n;
using Frapid.i18n.Database;
using Frapid.i18n.Models;

namespace Frapid.Web.Controllers
{
    public class ResourceController : FrapidController
    {
        [Route("i18n/resources.js")]
        [OutputCache(Duration = 31536000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Index()
        {
            string culture = CultureManager.GetCurrent().TwoLetterISOLanguageName;
            string script = GetScript(culture);
            return Content(script, "text/javascript", Encoding.UTF8);
        }

        private static string GetScript(string culture)
        {
            StringBuilder script = new StringBuilder();
            script.Append("var Resources = {");

            IEnumerable<LocalizedResource> resources = DbResources.GetLocalizationTable(culture);

            List<List<LocalizedResource>> resourceClassGroup = resources
                .GroupBy(r => r.ResourceClass)
                .Select(group => group.ToList())
                .ToList();

            foreach (List<LocalizedResource> resourceClass in resourceClassGroup)
            {
                int i = 0;

                foreach (LocalizedResource resource in resourceClass)
                {
                    if (i == 0)
                    {
                        script.Append(resource.ResourceClass + ": {");
                    }

                    script.Append(resource.Key + ": function(){ return \"");
                    string localized = resource.Translated;

                    if (string.IsNullOrWhiteSpace(localized))
                    {
                        localized = resource.Original;
                    }

                    script.Append(HttpUtility.JavaScriptStringEncode(localized));

                    script.Append("\";");
                    script.Append("},");
                    i++;
                }

                script.Append("},");
            }


            script.Append("};");

            return script.ToString();
        }
    }
}