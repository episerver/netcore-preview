using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Tracking.Commerce.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Reference.Commerce.Site.Features.Recommendations.Services
{
    public interface IRecommendationService
    {
        Task<TrackingResponseData> TrackProductAsync(HttpContext httpContext, string productCode, bool skipRecommendations);
        Task<TrackingResponseData> TrackSearchAsync(HttpContext httpContext, string searchTerm, IEnumerable<string> productCodes, int totalRecordsCount);
        Task<TrackingResponseData> TrackOrderAsync(HttpContext httpContext, IPurchaseOrder order);
        Task<TrackingResponseData> TrackCategoryAsync(HttpContext httpContext, NodeContent category);
        Task<TrackingResponseData> TrackCartAsync(HttpContext httpContext);
        Task<TrackingResponseData> TrackCartAsync(HttpContext httpContext, IEnumerable<CartChangeData> cartChanges);
        Task<TrackingResponseData> TrackWishlistAsync(HttpContext httpContext);
        Task<TrackingResponseData> TrackCheckoutAsync(HttpContext httpContext);

        IEnumerable<RecommendedProductTileViewModel> GetRecommendedProductTileViewModels(IEnumerable<Recommendation> recommendations);
    }
}