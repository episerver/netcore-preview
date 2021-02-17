using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.AddressBook.Controllers
{
    [ControllerExceptionFilter("save")]
    [Authorize]
    [AllowAnonymous]
    public class AddressBookController : PageController<AddressBookPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IAddressBookService _addressBookService;
        private readonly LocalizationService _localizationService;

        public AddressBookController(
            IContentLoader contentLoader,
            IAddressBookService addressBookService,
            LocalizationService localizationService)
        {
            _contentLoader = contentLoader;
            _addressBookService = addressBookService;
            _localizationService = localizationService;
        }

        [HttpGet]
        public ActionResult Index(AddressBookPage currentPage)
        {
            var viewModel = _addressBookService.GetAddressBookViewModel(currentPage);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult EditForm(AddressBookPage currentPage, string addressId)
        {
            var viewModel = new AddressViewModel
            {
                Address = new AddressModel
                {
                    AddressId = addressId,
                },
                CurrentPage = currentPage
            };

            _addressBookService.LoadAddress(viewModel.Address);

            return AddressEditView(viewModel);
        }

        [HttpPost]
        public IActionResult GetRegionsForCountry(string countryCode, string region, string htmlPrefix)
        {
            ViewData.TemplateInfo.HtmlFieldPrefix = htmlPrefix;
            var countryRegion = new CountryRegionViewModel
            {
                RegionOptions = _addressBookService.GetRegionsByCountryCode(countryCode),
                Region = region
            };

            return PartialView("~/Views/Shared/EditorTemplates/AddressRegion.cshtml", countryRegion);
        }

        [HttpPost]
        public IActionResult Save(AddressViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.Address.Name))
            {
                ModelState.AddModelError("Address.Name", _localizationService.GetString("/Shared/Address/Form/Empty/Name"));
            }

            if (!_addressBookService.CanSave(viewModel.Address))
            {
                ModelState.AddModelError("Address.Name", _localizationService.GetString("/AddressBook/Form/Error/ExistingAddress"));
            }

            if (!ModelState.IsValid)
            {
                _addressBookService.LoadAddress(viewModel.Address);

                return AddressEditView(viewModel);
            }

            _addressBookService.Save(viewModel.Address);

            if (Request.IsAjaxRequest())
            {
                return Json(viewModel.Address);
            }

            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult Remove(string addressId)
        {
            _addressBookService.Delete(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult SetPreferredShippingAddress(string addressId)
        {
            _addressBookService.SetPreferredShippingAddress(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        [HttpPost]
        public ActionResult SetPreferredBillingAddress(string addressId)
        {
            _addressBookService.SetPreferredBillingAddress(addressId);
            return RedirectToAction("Index", new { node = GetStartPage().AddressBookPage });
        }

        private ActionResult AddressEditView(AddressViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("ModalAddressDialog", viewModel);
            }

            return View("EditForm", viewModel);
        }

        private StartPage GetStartPage()
        {
            return _contentLoader.Get<StartPage>(ContentReference.StartPage);
        }
    }
}