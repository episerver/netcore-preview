using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    [AllowAnonymous]
    public class CategoryPartialController : PartialContentComponent<NodeContent>
    {
        private readonly SearchViewModelFactory _viewModelFactory;

        public CategoryPartialController(SearchViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        [HttpGet]
        [HttpPost]
        protected override IViewComponentResult InvokeComponent(NodeContent currentContent)
        {
            var productModels = GetProductModels(currentContent);
            return View("_Category", productModels);
        }

        protected IEnumerable<ProductTileViewModel> GetProductModels(NodeContent currentContent)
        {
            return GetSearchModel(currentContent, 3).ProductViewModels;
        }

        protected virtual SearchViewModel<NodeContent> GetSearchModel(NodeContent currentContent, int pageSize)
        {
            return _viewModelFactory.Create(currentContent, new FilterOptionViewModel
            {
                FacetGroups = new List<FacetGroupOption>(),
                Page = 1,
                PageSize = pageSize
            });
        }
    }
}