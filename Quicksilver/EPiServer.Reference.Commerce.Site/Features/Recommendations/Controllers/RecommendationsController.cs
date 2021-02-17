using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Recommendations.Controllers
{
    public class RecommendationsController : ViewComponent
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public IViewComponentResult Invoke(IEnumerable<Recommendation> recommendations)
        {
            if (recommendations == null || !recommendations.Any())
            {
                return new ContentViewComponentResult(string.Empty);
            }

            var viewModel = new RecommendationsViewModel
            {
                Products = _recommendationService.GetRecommendedProductTileViewModels(recommendations)
            };

            return View("_Recommendations", viewModel);
        }
    }
}