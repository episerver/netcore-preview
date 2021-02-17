using EPiServer.Reference.Commerce.Site.Features.Campaign.Pages;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Campaign.Controllers
{
    public class CampaignController : PageController<CampaignPage>
    {
        [HttpGet]
        public IActionResult Index(CampaignPage currentPage)
        {
            return View(currentPage);
        }
    }
}