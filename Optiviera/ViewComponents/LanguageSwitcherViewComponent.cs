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
                { "tr", "🇹🇷 Türkçe" },
                { "en", "🇺🇸 English" },
                { "de", "🇩🇪 Deutsch" },
                { "fr", "🇫🇷 Français" },
                { "es", "🇪🇸 Español" },
                { "it", "🇮🇹 Italiano" },
                { "pt", "🇵🇹 Português" },
                { "nl", "🇳🇱 Nederlands" }
            };
            
            var currentCulture = HttpContext.Features.Get<IRequestCultureFeature>()
                ?.RequestCulture.Culture.Name ?? "tr";
            
            ViewBag.CurrentCulture = currentCulture;
            ViewData["CurrentCulture"] = currentCulture;
            return View(languages);
        }
    }
}
