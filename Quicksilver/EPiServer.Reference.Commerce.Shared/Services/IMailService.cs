using EPiServer.Core;
using Microsoft.AspNetCore.Identity;
using System.Collections.Specialized;
using System.Net.Mail;

namespace EPiServer.Reference.Commerce.Shared.Services
{
    public interface IMailService
    {
        void Send(ContentReference mailReference, NameValueCollection nameValueCollection, string toEmail, string language);
        void Send(string subject, string body, string toEmail);
        void Send(MailMessage message);
        string GetHtmlBodyForMail(ContentReference mailReference, NameValueCollection nameValueCollection, string langauge);
    }
}