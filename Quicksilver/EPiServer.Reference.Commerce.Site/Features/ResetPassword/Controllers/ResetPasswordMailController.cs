using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.Login.Services;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.Pages;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.ResetPassword.Controllers
{
    public class ResetPasswordMailController : IdentityControllerBase<ResetPasswordMailPage>
    {
        public ResetPasswordMailController(ApplicationSignInManager<ApplicationUser> signinManager, ApplicationUserManager<ApplicationUser> userManager, UserService userService)
            : base(signinManager, userManager, userService)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index(ResetPasswordMailPage currentPage, string language)
        {
            var viewModel = new ResetPasswordMailPageViewModel { CurrentPage = currentPage };
            return await Task.FromResult(View(viewModel));
        }
    }
}