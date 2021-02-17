using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    [AllowAnonymous]
    public class ProductController : ContentController<FashionProduct>
    {
        private readonly CatalogEntryViewModelFactory _viewModelFactory;
        private readonly IContextModeResolver _contextModeResolver;

        public ProductController(CatalogEntryViewModelFactory viewModelFactory, IContextModeResolver contextModeResolver)
        {
            _viewModelFactory = viewModelFactory;
            _contextModeResolver = contextModeResolver;
        }

        [HttpGet]
        public ActionResult Index(FashionProduct currentContent, string entryCode = "", bool useQuickview = false, bool skipTracking = false)
        {
            var viewModel = _viewModelFactory.Create(currentContent, entryCode);
            viewModel.SkipTracking = skipTracking;

            if (_contextModeResolver.CurrentMode == ContextMode.Edit && viewModel.Variant == null)
            {
                var emptyViewName = "ProductWithoutEntries";
                return Request.IsAjaxRequest() ? PartialView(emptyViewName, viewModel) : (ActionResult)View(emptyViewName, viewModel);
            }

            if (viewModel.Variant == null)
            {
                return NotFound();
            }

            if (useQuickview)
            {
                return PartialView("_Quickview", viewModel);
            }
            return Request.IsAjaxRequest() ? PartialView(viewModel) : (ActionResult)View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectVariant(FashionProduct currentContent, string color, string size, bool useQuickview = false)
        {
            var variant = _viewModelFactory.SelectVariant(currentContent, color, size);
            if (variant != null)
            {
                var viewModel = _viewModelFactory.Create(currentContent, variant.Code);

                viewModel.SkipTracking = true;

                if (_contextModeResolver.CurrentMode == ContextMode.Edit && viewModel.Variant == null)
                {
                    var emptyViewName = "ProductWithoutEntries";
                    return Request.IsAjaxRequest() ? PartialView(emptyViewName, viewModel) : (ActionResult)View(emptyViewName, viewModel);
                }

                if (viewModel.Variant == null)
                {
                    return NotFound();
                }

                if (useQuickview)
                {
                    return PartialView("_Quickview", viewModel);
                }

                return Request.IsAjaxRequest() ? PartialView("Index", viewModel) : (ActionResult)View("Index", viewModel);
            }
            return NotFound();
        }
    }
}