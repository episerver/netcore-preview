using EPiServer.Authorization;
using EPiServer.Shell.Security;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public class UsersInstaller : IBlockingFirstRequestInitializer
    {
        private readonly UIUserProvider _uIUserProvider;
        private readonly UIRoleProvider _uIRoleProvider;
        private readonly UISignInManager _uISignInManager;

        public UsersInstaller(UIUserProvider uIUserProvider, 
            UISignInManager uISignInManager, 
            UIRoleProvider uIRoleProvider)
        {
            _uIUserProvider = uIUserProvider;
            _uISignInManager = uISignInManager;
            _uIRoleProvider = uIRoleProvider;
        }

        public bool CanRunInParallel => false;

        public async Task InitializeAsync(HttpContext httpContext) 
        {
            if (await IsAnyUserRegistered())
            {
                return;
            }

            await CreateUser("admin@example.com", "admin@example.com", new[] { Roles.Administrators, Roles.WebAdmins });
        }

        private async Task CreateUser(string username, string email, IEnumerable<string> roles)
        {
            var result = await _uIUserProvider.CreateUserAsync(username, "Episerver123!", email, null, null, true);
            if (result.Status == UIUserCreateStatus.Success)
            {
                foreach(var role in roles)
                {
                    var exists = await _uIRoleProvider.RoleExistsAsync(role);
                    if (!exists)
                    {
                        await _uIRoleProvider.CreateRoleAsync(role);
                    }
                }

                await _uIRoleProvider.AddUserToRolesAsync(result.User.Username, roles);
                var resFromSignIn = await _uISignInManager.SignInAsync(username, "Episerver123!");
            }
        }

        private async Task<bool> IsAnyUserRegistered()
        {
            var res = await _uIUserProvider.GetAllUsersAsync(0, 1).CountAsync();
            return res > 0;
        }
    }
}
