using System.Collections.Generic;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Reference.Commerce.Site.Features.Market.ViewModels
{
    public class MarketViewModel
    {
        public IEnumerable<SelectListItem> Markets { get; set; }
        public string MarketId { get; set; }
        public ContentReference ContentLink { get; set; }
    }
}