using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Services;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class VariationPartialController : PartialContentComponent<VariationContent>
    {
        private readonly IProductService _productService;

        public VariationPartialController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [HttpPost]
        protected override IViewComponentResult InvokeComponent(VariationContent currentContent)
        {
            return View("_Product", _productService.GetProductTileViewModel(currentContent));
        }
    }
}