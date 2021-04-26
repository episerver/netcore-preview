using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Pages
{
    public class MailBasePage : PageData
    {
        [CultureSpecific]
        [Display(
            Name = "Mail Title",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string MailTitle { get; set; }
    }
}