using System.Collections.Generic;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Reference.Commerce.Site.Features.Market.ViewModels
{
    public class LanguageViewModel
    {
        public IEnumerable<SelectListItem> Languages { get; set; }
        public string Language { get; set; }
        public ContentReference ContentLink { get; set; }
    }
}