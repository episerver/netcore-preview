using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    [AllowAnonymous]
    public class CategoryController : ContentController<FashionNode>
    {
        private readonly SearchViewModelFactory _viewModelFactory;

        public CategoryController(SearchViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Index(FashionNode currentContent, FilterOptionViewModel viewModel)
        {
            var model = _viewModelFactory.Create(currentContent, viewModel);

            return View(model);
        }
    }
}