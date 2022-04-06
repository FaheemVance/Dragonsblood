using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DragonsBlood.Data;
using Microsoft.AspNet.Identity.EntityFramework;
using DragonsBlood.Data.Session;
using System.Collections.Generic;
using DragonsBlood.Models.Roles;
using DragonsBlood.Models.Users;

namespace DragonsBlood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        
        public MembersController()
        {
            this.SetSession();
        }
        
        // GET: Members
        public ActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public ActionResult RestrictMember(string Id)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
            {
                HttpContext.AddError(new System.Exception("User could not be located"));
                return RedirectToAction("Index");
            }

            manager.RemoveFromRoles(Id, "Member");
            manager.RemoveFromRoles(Id, "Moderator");
            manager.RemoveFromRoles(Id, "Leader");
            manager.AddToRole(Id, "Restricted");

            return RedirectToAction("Index");
        }

        public ActionResult RestrictChat(string Id)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
            {
                HttpContext.AddError(new System.Exception("User could not be located"));
                return RedirectToAction("Index");
            }

            manager.RemoveFromRoles(Id, "ChatUser");

            UpdateUser(Id);

            return RedirectToAction("Index");
        }

        public ActionResult UnRestrictMember(string Id)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
                return RedirectToAction("Index");

            var r = manager.RemoveFromRoles(Id, "Restricted");
            var r2 = manager.AddToRole(Id, "Member");

            return RedirectToAction("Index");
        }

        public ActionResult AddChat(string Id)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
            {
                HttpContext.AddError(new System.Exception("User could not be located"));
                return RedirectToAction("Index");
            }
            var r = manager.AddToRole(Id, "ChatUser");
            UpdateUser(Id);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteUser(string Id)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
            {
                HttpContext.AddError(new System.Exception("User could not be located"));
                return RedirectToAction("Index");
            }

            var r = manager.Delete(user);

            return RedirectToAction("Index");
        }

        public ActionResult ChangeDisplayName(string Id, string name)
        {
            var manager = GetUserManager();
            var user = manager.FindById(Id);

            if (user == null)
            {
                HttpContext.AddError(new System.Exception("User could not be located"));
                return RedirectToAction("Index");
            }

            if(string.IsNullOrEmpty(name))
                return RedirectToAction("Index");

            var r = manager.ChangeDisplayName(Id, name);

            return RedirectToAction("Index");
        }

        private ApplicationUserManager GetUserManager(ApplicationDbContext context = null)
        {
            if (context == null)
                context = new ApplicationDbContext();
            return new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        private ApplicationRoleManager GetRoleManager(ApplicationDbContext context = null)
        {
            if (context == null)
                context = new ApplicationDbContext();

            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));
        }

        private ICollection<ApplicationUserRole> GetUserRolesForUser(string userId)
        {
            var userManager = GetUserManager();
            var roleManager = GetRoleManager();

            var user = userManager.FindById(userId);
            


            return null;
        }

        private void UpdateUser(string userId)
        {
            //var manager = new UserHub();
            //manager.LogUserOut(userId);
        }
    }
}