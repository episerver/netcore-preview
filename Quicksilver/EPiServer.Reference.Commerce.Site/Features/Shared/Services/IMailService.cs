using EPiServer.Core;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Collections.Specialized;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    public interface IMailService
    {
        void Send(ContentReference mailReference, NameValueCollection nameValueCollection, string toEmail, string language);
        void Send(string subject, string body, string toEmail);
        void Send(MimeMessage message);
        string GetHtmlBodyForMail(ContentReference mailReference, NameValueCollection nameValueCollection, string langauge);
    }
}