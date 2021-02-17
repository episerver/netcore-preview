using EPiServer.Cms.AspNetCore.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Folder.Editorial.Blocks;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Folder.Editorial.Controllers
{
    public class FreeTextBlockController : BlockComponent<FreeTextBlock>
    {
        [HttpGet]
        [HttpPost]
        public override IViewComponentResult Invoke(FreeTextBlock currentBlock)
        {
            return View(currentBlock);
        }
    }
}