using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Services;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class ProductPartialController : PartialContentComponent<ProductContent>
    {
        private readonly IProductService _productService;

        public ProductPartialController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [HttpPost]
        public override IViewComponentResult Invoke(ProductContent currentContent)
        {
            return View("_Product", _productService.GetProductTileViewModel(currentContent));
        }
    }
}