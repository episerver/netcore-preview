using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public class PackageController : ContentController<FashionPackage>
    {
        private readonly CatalogEntryViewModelFactory _viewModelFactory;
        private readonly IContextModeResolver _contextModeResolver;

        public PackageController(CatalogEntryViewModelFactory viewModelFactory, IContextModeResolver contextModeResolver)
        {
            _viewModelFactory = viewModelFactory;
            _contextModeResolver = contextModeResolver;
        }

        [HttpGet]
        public ActionResult Index(FashionPackage currentContent, bool useQuickview = false)
        {
            var viewModel = _viewModelFactory.Create(currentContent);

            if (_contextModeResolver.CurrentMode == ContextMode.Edit && !viewModel.Entries.Any())
            {
                var emptyViewName = "PackageWithoutEntries";
                return Request.IsAjaxRequest() ? PartialView(emptyViewName, viewModel) : (ActionResult)View(emptyViewName, viewModel);
            }

            if (useQuickview)
            {
                return PartialView("_Quickview", viewModel);
            }
            return Request.IsAjaxRequest() ? PartialView(viewModel) : (ActionResult)View(viewModel);
        }
    }
}