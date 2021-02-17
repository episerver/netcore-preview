using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.ViewModels;
using Mediachase.Commerce;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class FashionProductViewModel : ProductViewModelBase
    {
        public FashionProduct Product { get; set; }
        public Money? DiscountedPrice { get; set; }
        public Money ListingPrice { get; set; }
        public FashionVariant Variant { get; set; }
        public IList<SelectListItem> Colors { get; set; }
        public IList<SelectListItem> Sizes { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public bool IsAvailable { get; set; }
    }
}