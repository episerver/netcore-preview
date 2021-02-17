using EPiServer.Reference.Commerce.Site.Features.Search.Pages;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    [AllowAnonymous]
    public class SearchController : PageController<SearchPage>
    {
        private readonly SearchViewModelFactory _viewModelFactory;
        private readonly ISearchService _searchService;

        public SearchController(
            SearchViewModelFactory viewModelFactory, 
            ISearchService searchService)
        {
            _viewModelFactory = viewModelFactory;
            _searchService = searchService;
        }

        [HttpGet]
        [HttpPost]
        public ActionResult Index(SearchPage currentPage, FilterOptionViewModel filterOptions)
        {
            var viewModel = _viewModelFactory.Create(currentPage, filterOptions);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult QuickSearch(string q = "")
        {
            var result = _searchService.QuickSearch(q);
            return View("_QuickSearch", result);
        }
    }
}