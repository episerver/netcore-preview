using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloyMvcTemplates.Models;
using EPiServer.Shell.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlloyMvcTemplates.Pages
{
    public class LoginModel : PageModel
    {
        UISignInManager _uISignInManager;
        UIUserProvider _uIUserProvider;
        private const string errorMsg = "Login failed. Your username or password is incorrect";

        public LoginModel(UISignInManager uISignInManager, UIUserProvider uIUserProvider)
        {
            _uISignInManager = uISignInManager;
            _uIUserProvider = uIUserProvider;
        }

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                ModelState.AddModelError("error", errorMsg);
                return Page();
            }

            var resFromSignIn = await _uISignInManager.SignInAsync(_uIUserProvider.Name, LoginViewModel.Username, LoginViewModel.Password);
            if (resFromSignIn)
            {
                return Redirect("/");
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("error", errorMsg);

                return Page();
            }
        }
    }
}
