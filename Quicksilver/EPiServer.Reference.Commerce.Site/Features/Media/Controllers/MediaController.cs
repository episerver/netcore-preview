using EPiServer.Cms.AspNetCore.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Media.Controllers
{
    [TemplateDescriptor(TemplateTypeCategory = TemplateTypeCategories.MvcPartialComponent, Inherited = true)]
    public class MediaController : PartialContentComponent<ImageMediaData>
    {
        protected override IViewComponentResult InvokeComponent(ImageMediaData currentContent)
        {
            return View(currentContent);
        }
    }
}