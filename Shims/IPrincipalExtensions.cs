using System.Security.Principal;

namespace EPiServer.Web.Mvc
{
    /// <summary>
    /// this is a dummy impl. We don't know how we should take care of edit access, before was in the web config with allow/deny which iis handle it
    /// in this case maybe we need to have a claim or ... 
    /// </summary>
    public static class IPrincipalExtensions
    {
        private const string editrole = "ep_edit_role";
        public static bool HasEditAccess(this IPrincipal principal) => principal.IsInRole(editrole);
    }

}