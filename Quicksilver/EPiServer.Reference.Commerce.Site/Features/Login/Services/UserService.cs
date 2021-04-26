using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Login.Services
{
    [ServiceConfiguration]
    public class UserService : IDisposable
    {
        private readonly UIUserProvider _uiUserProvider;
        private readonly UISignInManager _uISignInManager;
        private readonly ApplicationUserManager<ApplicationUser> _applicationUserManager;
        private readonly LocalizationService _localizationService;
        private readonly CustomerContextFacade _customerContext;

        public UserService(UIUserProvider uiUserProvider,
            UISignInManager uiSignInManager,
            ApplicationUserManager<ApplicationUser> applicationUserManager,
            LocalizationService localizationService,
            CustomerContextFacade customerContextFacade)
        {
            _uiUserProvider = uiUserProvider;
            _uISignInManager = uiSignInManager;
            _applicationUserManager = applicationUserManager;
            _localizationService = localizationService;
            _customerContext = customerContextFacade;
        }

        public async virtual Task<CustomerContact> GetCustomerContact(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            CustomerContact contact = null;
            var user = await _applicationUserManager.FindByEmailAsync(email);

            if (user != null)
            {
                contact = _customerContext.GetContactById(new Guid(user.Id));
            }

            return contact;
        }

        public virtual async Task<IUIUser> GetUserAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await _uiUserProvider.GetUserAsync(email);
        }

        public virtual async Task<ContactIdentityResult> RegisterAccount(SiteUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrEmpty(user.Password))
            {
                throw new MissingFieldException("Password");
            }

            if (String.IsNullOrEmpty(user.Email))
            {
                throw new MissingFieldException("Email");
            }

            CreateUserResult result;
            CustomerContact contact = null;

            if (await _uiUserProvider.GetUserAsync(user.Email) != null)
            {
                result = new CreateUserResult(null, UIUserCreateStatus.DuplicateEmail, new[]{ _localizationService.GetString("/Registration/Form/Error/UsedEmail") });
            }
            else
            {
                result = await _uiUserProvider.CreateUserAsync(user.UserName, user.Password, user.Email, null, null, true);

                if (result.Status == UIUserCreateStatus.Success)
                {
                    contact = await CreateCustomerContact(user);
                }
            }

            var contactResult = new ContactIdentityResult(result, contact);

            return contactResult;
        }

        public async Task<CustomerContact> CreateCustomerContact(SiteUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            CustomerContact contact = _customerContext.GetContactByUsername(user.UserName);
            if (contact == null)
            {
                contact = CustomerContact.CreateInstance();
                var appUser = await _applicationUserManager.FindByEmailAsync(user.Email);
                contact.PrimaryKeyId = new PrimaryKeyId(new Guid(appUser.Id));
                contact.UserId = "String:" + user.Email; // The UserId needs to be set in the format "String:{email}". Else a duplicate CustomerContact will be created later on.
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            if (!String.IsNullOrEmpty(user.FirstName) || !String.IsNullOrEmpty(user.LastName))
            {
                contact.FullName = $"{user.FirstName} {user.LastName}";
            }

            contact.FirstName = user.FirstName;
            contact.LastName = user.LastName;
            contact.Email = user.Email;
            contact.RegistrationSource = user.RegistrationSource;

            if (user.Addresses != null)
            {
                foreach (var address in user.Addresses)
                {
                    contact.AddContactAddress(address);
                }
            }

            // The contact, or more likely its related addresses, must be saved to the database before we can set the preferred
            // shipping and billing addresses. Using an address id before its saved will throw an exception because its value
            // will still be null.
            contact.SaveChanges();

            SetPreferredAddresses(contact);

            return contact;
        }

        public void Dispose()
        {
            _uiUserProvider?.Dispose();
            _uISignInManager?.Dispose();
            _applicationUserManager?.Dispose();
        }

        public async Task SignOut()
        {
            await _uISignInManager.SignOutAsync();
        }

        private void SetPreferredAddresses(CustomerContact contact)
        {
            var changed = false;

            var publicAddress = contact.ContactAddresses.FirstOrDefault(a => a.AddressType == CustomerAddressTypeEnum.Public);
            var preferredBillingAddress = contact.ContactAddresses.FirstOrDefault(a => a.AddressType == CustomerAddressTypeEnum.Billing);
            var preferredShippingAddress = contact.ContactAddresses.FirstOrDefault(a => a.AddressType == CustomerAddressTypeEnum.Shipping);

            if (publicAddress != null)
            {
                contact.PreferredShippingAddress = contact.PreferredBillingAddress = publicAddress;
                changed = true;
            }

            if (preferredBillingAddress != null)
            {
                contact.PreferredBillingAddress = preferredBillingAddress;
                changed = true;
            }

            if (preferredShippingAddress != null)
            {
                contact.PreferredShippingAddress = preferredShippingAddress;
                changed = true;
            }

            if (changed)
            {
                contact.SaveChanges();
            }
        }
    }
}