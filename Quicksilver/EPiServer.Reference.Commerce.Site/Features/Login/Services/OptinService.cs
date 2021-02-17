using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data.Dynamic;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Login.Services
{
    internal class OptinTokenData : IDynamicData
    {
        public Data.Identity Id { get; set; }
        public string UserId { get; set; }
        public string OptinConfirmationToken { get; set; }
        public DateTime Created { get; set; }
    }

    [ServiceConfiguration]
    public class OptinService : IDisposable
    {
        private const string MarketingEmailOptinPurpose = "Marketing email opt-in confirmation";
        private readonly DynamicDataStoreFactory _storeFactory;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly CustomerContextFacade _customerContext;

        public OptinService(DynamicDataStoreFactory storeFactory, ApplicationUserManager<ApplicationUser> userManager, CustomerContextFacade customerContext)
        {
            _storeFactory = storeFactory;
            _userManager = userManager;
            _customerContext = customerContext;
        }

        private DynamicDataStore TokenStore => _storeFactory.GetStore(typeof(OptinTokenData)) ??
                                               _storeFactory.CreateStore(typeof(OptinTokenData));

        public void Dispose()
        {
            _userManager?.Dispose();
        }

        public virtual async Task<string> CreateOptinTokenData(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var token = await _userManager.GenerateUserTokenAsync(user, "optintokenprovider", MarketingEmailOptinPurpose);
            var tokenData = new OptinTokenData
            {
                UserId = user.Id,
                OptinConfirmationToken = token,
                Created = DateTime.Now
            };

            TokenStore.Save(tokenData);
            return tokenData.OptinConfirmationToken;
        }

        public async Task<bool> ConfirmOptinToken(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var userOptinTokenData = TokenStore.Find<OptinTokenData>("UserId", user.Id).FirstOrDefault();
            if (userOptinTokenData == null)
            {
                return false;
            }

            var confirmResult = await _userManager.VerifyUserTokenAsync(user, "optintokenprovider", MarketingEmailOptinPurpose, token);
            if (!confirmResult)
            {
                return false;
            }

            // Update consent data to CustomerContact
            var contact = _customerContext.GetContactByUsername(user.UserName);
            UpdateOptin(contact, true);

            // Delete token data from DDS
            TokenStore.Delete(userOptinTokenData.Id);
            return true;
        }

        public ConsentData GetCurrentContactConsentData()
        {
            var currentContact = GetCurrentContact();
            return new ConsentData
            {
                AcceptMarketingEmail = currentContact.AcceptMarketingEmail,
                ConsentUpdated = currentContact.ConsentUpdated
            };
        }

        public void UpdateOptinForCurrentContact(bool acceptMarketingEmail)
        {
            var currentContact = GetCurrentContact();
            UpdateOptin(currentContact, acceptMarketingEmail);
        }

        private void UpdateOptin(CustomerContact contact, bool acceptMarketingEmail)
        {
            if (contact.AcceptMarketingEmail != acceptMarketingEmail)
            {
                contact.AcceptMarketingEmail = acceptMarketingEmail;
                contact.ConsentUpdated = DateTime.Now;
                contact.SaveChanges();
            }
        }

        private CustomerContact GetCurrentContact()
        {
            return _customerContext.CurrentContact.CurrentContact;
        }
    }
}