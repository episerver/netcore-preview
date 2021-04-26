using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Reference.Commerce.Site.Features.Shared.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Specialized;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    [ServiceConfiguration(typeof(IMailService), Lifecycle = ServiceInstanceScope.Transient)]
    public class MailService : IMailService
    {
        private const int DefaultSmtpPort = 587;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UrlResolver _urlResolver;
        private readonly IContentLoader _contentLoader;
        private readonly IHtmlDownloader _htmlDownloader;
        private readonly IOptions<SmtpOptions> _smtpOptions;
        private readonly IConfiguration _configuration;

        public MailService(IHttpContextAccessor httpContextBase, 
            UrlResolver urlResolver, 
            IContentLoader contentLoader,
            IHtmlDownloader htmlDownloader, 
            IOptions<SmtpOptions> smtpOptions,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextBase;
            _urlResolver = urlResolver;
            _contentLoader = contentLoader;
            _htmlDownloader = htmlDownloader;
            _smtpOptions = smtpOptions;
            _configuration = configuration;
        }

        public void Send(ContentReference mailReference, NameValueCollection nameValueCollection, string toEmail, string language)
        {
            var body = GetHtmlBodyForMail(mailReference, nameValueCollection, language);
            var mailPage = _contentLoader.Get<MailBasePage>(mailReference);

            Send(mailPage.MailTitle, body, toEmail);
        }

        public string GetHtmlBodyForMail(ContentReference mailReference, NameValueCollection nameValueCollection, string language)
        {
            var urlBuilder = new UrlBuilder(_urlResolver.GetUrl(mailReference, language))
            {
                QueryCollection = nameValueCollection
            };

            var basePath = new Uri(_httpContextAccessor.HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
            var relativePath = urlBuilder.ToString();
            
            if (relativePath.StartsWith(basePath))
            {
                relativePath = relativePath.Substring(basePath.Length);
            }

            return _htmlDownloader.Download(basePath, relativePath);
        }

        public void Send(string subject, string body, string recipientMailAddress)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart("html") { Text = body }
            };
            message.From.Add(MailboxAddress.Parse(_configuration.GetValue<string>("EPiServer:SmtpOptions:FromEmailAddress")));
            message.To.Add(MailboxAddress.Parse(recipientMailAddress));

            Send(message);
        }

        public void Send(MimeMessage message)
        {
            int port = _smtpOptions.Value.Network.Port.HasValue ? _smtpOptions.Value.Network.Port.Value : DefaultSmtpPort;
            bool useSsl = _smtpOptions.Value.Network.UseSsl.HasValue? _smtpOptions.Value.Network.UseSsl.Value : false;

            using var client = new SmtpClient();
            client.Connect(_smtpOptions.Value.Network.Host, port, useSsl);
            client.Authenticate(_smtpOptions.Value.Network.UserName, _smtpOptions.Value.Network.Password);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}