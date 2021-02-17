using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly LocalizationService _localizationService;
        private readonly CartViewModelFactory _cartViewModelFactory;

        public NavigationViewComponent(
            IContentLoader contentLoader,
            ICartService cartService,
            IUrlHelperFactory urlHelperFactory,
            LocalizationService localizationService,
            CartViewModelFactory cartViewModelFactory)
        {
            _contentLoader = contentLoader;
            _cartService = cartService;
            _urlHelperFactory = urlHelperFactory;
            _localizationService = localizationService;
            _cartViewModelFactory = cartViewModelFactory;
        }

        public IViewComponentResult Invoke(IContent currentContent)
        {
            var cart = _cartService.LoadCart(_cartService.DefaultCartName);
            var wishlist = _cartService.LoadCart(_cartService.DefaultWishListName);
            var startPage = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            
            var viewModel = new NavigationViewModel
            {
                StartPage = startPage,
                CurrentContentLink = currentContent?.ContentLink,
                UserLinks = new LinkItemCollection(),
                MiniCart = _cartViewModelFactory.CreateMiniCartViewModel(cart),
                WishListMiniCart = _cartViewModelFactory.CreateWishListMiniCartViewModel(wishlist)
            };

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var rightMenuItems = startPage.RightMenu;
                if (rightMenuItems != null)
                {
                    viewModel.UserLinks.AddRange(rightMenuItems);
                }

                viewModel.UserLinks.Add(new LinkItem
                {
                    Href = _urlHelperFactory.GetUrlHelper(ViewContext).Action("SignOut", "Login"),
                    Text = _localizationService.GetString("/Header/Account/SignOut")
                });
            }
            else
            {
                viewModel.UserLinks.Add(new LinkItem
                {
                    Href = _urlHelperFactory.GetUrlHelper(ViewContext).Action("Index", "Login", new { returnUrl = currentContent != null ? _urlHelperFactory.GetUrlHelper(ViewContext).ContentUrl(currentContent.ContentLink) : "/" }),
                    Text = _localizationService.GetString("/Header/Account/SignIn")
                });
            }

            return View(viewModel);
        }
    }
}
