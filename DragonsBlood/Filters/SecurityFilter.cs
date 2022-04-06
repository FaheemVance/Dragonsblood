using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Models.Roles;
using DragonsBlood.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.Filters
{
    public class SecurityFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            using (var context = new ApplicationDbContext())
            {
                var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));
                var user = context.Users.FirstOrDefault(f => f.UserName == filterContext.HttpContext.User.Identity.Name);

                if (user == null)
                {
                    base.OnResultExecuting(filterContext);
                    return;
                }

                var usersRoles =
                    context.Users.Include(u => u.Roles)
                        .ToList()
                        .Single(u => u.UserName == filterContext.HttpContext.User.Identity.Name).Roles.Select(r => roleManager.FindById(r.RoleId).Name).ToList();

                var allRoles = roleManager.Roles.ToList();

                bool upToDate = true;

                foreach (var role in allRoles)
                {
                    if (!filterContext.HttpContext.User.IsInRole(role.Name) && usersRoles.Any(r => r == role.Name))
                        upToDate = false;
                }

                if (!upToDate)
                {
                    //Get the authentication manager
                    var authenticationManager = filterContext.HttpContext.GetOwinContext().Authentication;

                    //Log the user out
                    authenticationManager.SignOut();

                    //Log the user back in
                    var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties() { IsPersistent = true }, identity);
                }
            }

            
            base.OnResultExecuting(filterContext);
        }
    }
}