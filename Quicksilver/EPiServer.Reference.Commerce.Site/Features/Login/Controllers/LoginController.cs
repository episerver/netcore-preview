using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Login.Pages;
using EPiServer.Reference.Commerce.Site.Features.Login.Services;
using EPiServer.Reference.Commerce.Site.Features.Login.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Shared.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using Mediachase.Commerce.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Login.Controllers
{
    [AllowAnonymous]
    public class LoginController : IdentityControllerBase<LoginRegistrationPage>
    {
        private readonly IAddressBookService _addressBookService;
        private readonly IContentLoader _contentLoader;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(LoginController));
        private readonly IMailService _mailService;
        private readonly LocalizationService _localizationService;
        private readonly OptinService _optinService;

        private StartPage StartPage => _contentLoader.Get<StartPage>(ContentReference.StartPage);
        
        public LoginController(
            ApplicationSignInManager<ApplicationUser> signinManager,
            ApplicationUserManager<ApplicationUser> userManager,
            UserService userService,
            IAddressBookService addressBookService,
            IContentLoader contentLoader,
            IMailService mailService,
            LocalizationService localizationService,
            OptinService optinService)
            : base(signinManager, userManager, userService)
        {
            _addressBookService = addressBookService;
            _contentLoader = contentLoader;
            _mailService = mailService;
            _localizationService = localizationService;
            _optinService = optinService;
        }

        /// <summary>
        /// Renders the default login page in which a user can both register a new account or log in
        /// to an existing one.
        /// </summary>
        /// <param name="returnUrl">The user's previous URL location. When logging in the user will be redirected back to this URL.</param>
        /// <returns>The default login and user account registration view.</returns>
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            var registrationPage = ContentReference.IsNullOrEmpty(StartPage.LoginRegistrationPage)
                ? new LoginRegistrationPage()
                : _contentLoader.Get<LoginRegistrationPage>(StartPage.LoginRegistrationPage);

            // Prevent open redirection attacks. Refer to: https://docs.microsoft.com/en-us/aspnet/mvc/overview/security/preventing-open-redirection-attacks
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            var viewModel = new LoginPageViewModel(registrationPage, returnUrl);
            viewModel.LoginViewModel.ResetPasswordPage = StartPage.ResetPasswordPage;

            _addressBookService.LoadAddress(viewModel.RegisterAccountViewModel.Address);
            viewModel.RegisterAccountViewModel.Address.Name = _localizationService.GetString("/Shared/Address/DefaultAddressName");

            return View(viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAccount(RegisterAccountViewModel viewModel)
        {
            if (viewModel.CurrentPage == null)
            {
                viewModel.CurrentPage = ContentReference.IsNullOrEmpty(StartPage.LoginRegistrationPage)
                ? new LoginRegistrationPage()
                : _contentLoader.Get<LoginRegistrationPage>(StartPage.LoginRegistrationPage);
            }

            if (!ModelState.IsValid)
            {
                _addressBookService.LoadAddress(viewModel.Address);
                return View(viewModel);
            }

            viewModel.Address.BillingDefault = true;
            viewModel.Address.ShippingDefault = true;
            viewModel.Address.Email = viewModel.Email;

            var customerAddress = CustomerAddress.CreateInstance();
            _addressBookService.MapToAddress(viewModel.Address, customerAddress);

            var user = new SiteUser
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                Password = viewModel.Password,
                FirstName = viewModel.Address.FirstName,
                LastName = viewModel.Address.LastName,
                RegistrationSource = "Registration page",
                AcceptMarketingEmail = viewModel.AcceptMarketingEmail,
                Addresses = new List<CustomerAddress>(new[] { customerAddress }),
            };

            var registration = await UserService.RegisterAccount(user);

            if (registration.Result.Status == Shell.Security.UIUserCreateStatus.Success)
            {
                if (user.AcceptMarketingEmail)
                {
                    var token = await _optinService.CreateOptinTokenData(registration.Contact.Email);
                    SendMarketingEmailConfirmationMail(registration.Contact.UserId, registration.Contact, token);
                }

                var signInResult = await SignInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, lockoutOnFailure: true);

                if (signInResult.Succeeded)
                {
                    return Json(new { ReturnUrl = GetSafeReturnUrl(Request.GetTypedHeaders().Referer) });
                }
            }

            _addressBookService.LoadAddress(viewModel.Address);

            AddErrors(registration.Result.Errors);

            return PartialView("RegisterAccount", viewModel);
        }

        private string GetSafeReturnUrl(Uri referrer)
        {
            //Make sure we only return to relative url.
            var returnUrl = HttpUtility.ParseQueryString(referrer.Query)["returnUrl"];

            // Prevent open redirection attacks. Refer to: https://docs.microsoft.com/en-us/aspnet/mvc/overview/security/preventing-open-redirection-attacks
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return "/";
            }

            return Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri) ? uri.PathAndQuery : returnUrl;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InternalLogin(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.ResetPasswordPage = StartPage.ResetPasswordPage;
                return PartialView("Login", viewModel);
            }

            var user = await UserService.GetUserAsync(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Password", _localizationService.GetString("/Login/Form/Error/WrongPasswordOrEmail"));
                viewModel.Password = null;

                return PartialView("Login", viewModel);
            }

            if (!user.IsApproved)
            {
                return PartialView("Unapproved", viewModel);
            }

            var result = await SignInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: true);


            if (result.Succeeded)
            {
                return Json(new { Success = true, ReturnUrl = GetSafeReturnUrl(Request.GetTypedHeaders().Referer) });
            }

            if (result.IsLockedOut)
            {
                return PartialView("Lockout", viewModel);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", "Login", new { ReturnUrl = viewModel.ReturnUrl, RememberMe = viewModel.RememberMe });
            }

            ModelState.AddModelError("Password", _localizationService.GetString("/Login/Form/Error/WrongPasswordOrEmail"));
            viewModel.Password = null;

            return PartialView("Login", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            throw new NotSupportedException();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            await Task.CompletedTask;
            throw new NotSupportedException();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel viewModel)
        {
            await Task.CompletedTask;
            throw new NotSupportedException();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmOptinToken(string userId, string token)
        {
            var user = await UserManager.FindByIdAsync(userId);

            var confirmResult = await _optinService.ConfirmOptinToken(user.Email, token);
            return confirmResult
                ? RedirectToAction("SuccessOptinConfirmation", "StandardPage")
                : RedirectToAction("PageNotFound", "ErrorHandling");
        }

        protected virtual void SendMarketingEmailConfirmationMail(string userId, CustomerContact contact, string token)
        {
            var optinConfirmEmailUrl = Url.Action("ConfirmOptinToken", "Login", new { userId, token }, protocol: Request.Scheme);
            try
            {
                var confirmationMailTitle = _contentLoader.Get<MailBasePage>(StartPage.RegistrationConfirmationMail).MailTitle;
                var confirmationMailBody = _mailService.GetHtmlBodyForMail(StartPage.RegistrationConfirmationMail, new NameValueCollection(), StartPage.Language.Name);
                confirmationMailBody = confirmationMailBody.Replace("[OptinConfirmEmailUrl]", optinConfirmEmailUrl);
                confirmationMailBody = confirmationMailBody.Replace("[Customer]", contact.LastName ?? contact.Email);
                _mailService.Send(confirmationMailTitle, confirmationMailBody, contact.Email);
            }
            catch (Exception e)
            {
                _logger.Warning($"Unable to send marketing email confirmation to '{contact.Email}'.", e);
            }
        }
    }
}