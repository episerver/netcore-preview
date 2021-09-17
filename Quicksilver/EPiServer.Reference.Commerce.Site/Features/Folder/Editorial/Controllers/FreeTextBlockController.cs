using EPiServer.Reference.Commerce.Site.Features.Folder.Editorial.Blocks;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Folder.Editorial.Controllers
{
    public class FreeTextBlockController : BlockComponent<FreeTextBlock>
    {
        [HttpGet]
        [HttpPost]
        protected override IViewComponentResult InvokeComponent(FreeTextBlock currentBlock)
        {
            return View(currentBlock);
        }
    }
}