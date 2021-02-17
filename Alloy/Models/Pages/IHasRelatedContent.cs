using EPiServer.Core;

namespace AlloyTemplates.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}