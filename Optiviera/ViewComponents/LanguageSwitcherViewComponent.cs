using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace Optiviera.ViewComponents
{
    public class LanguageSwitcherViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var languages = new Dictionary<string, string>
            {
                { "tr", "TR" },
                { "en", "EN" },
                { "de", "DE" },
                { "fr", "FR" },
                { "es", "ES" },
                { "it", "IT" },
                { "pt", "PT" },
                { "nl", "NL" }
            };
            
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>()
                ?.RequestCulture.Culture.Name ?? "tr";
            
            ViewBag.CurrentCulture = currentCulture;
            ViewData["CurrentCulture"] = currentCulture;
            return View(languages);
        }
    }
}
