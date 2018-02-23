using MVCShoppingCart.Models.Data;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVCShoppingCart
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            // Check if user is logged in
            if (User == null)
                return;

            // Get username
            string username = Context.User.Identity.Name;

            // Declare array of roles
            string[] roles = null;

            using (Db db = new Db())
            {
                // Populate roles
                var userDto = db.Users.FirstOrDefault(u => u.Username == username);
                roles = db.UserRoles
                    .Where(ur => ur.UserId == userDto.Id)
                    .Select(ur => ur.Role.Name)
                    .ToArray();

                // Build IPrincipal object
                var userIdentity = new GenericIdentity(username);
                GenericPrincipal userPrincipal = new GenericPrincipal(userIdentity, roles);

                // Update Context.User
                Context.User = userPrincipal;
            }

        }
    }
}
