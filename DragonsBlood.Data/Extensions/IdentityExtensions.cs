using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using DragonsBlood.Models.ChatModels;
using DragonsBlood.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.Data.Extensions
{
    public static class IdentityExtensions
    {
        public static string DisplayName(this IPrincipal ident)
        {
            var user = GetUser(ident);

            if(user != null)
            {
                return user.DisplayName;
            }

            return "User Not Found";
        }

        public static bool IsRestricted(this IPrincipal ident)
        {
            if (string.IsNullOrEmpty(ident.Identity.Name))
                return true;

            var user = GetUser(ident);

            if (user == null)
                return true;

            if (!ident.IsInRole("Restricted"))
                return false;

            return true;
        }

        public static bool IsInRole(this IPrincipal ident, string role)
        {
            var user = GetUser(ident);

            if (user == null)
                return false;

            if (!user.IsInRole(role))
                return false;

            return true;
        }

        public static bool IsAdmin(this IPrincipal ident)
        {
            var user = GetUser(ident);

            if (user == null)
                return false;

            if (!user.IsInRole("Admin"))
                return false;

            return true;
        }

        public static bool IsRestricted(this ApplicationUser user)
        {
            var manager = GetUserManager();

            var result = manager.IsInRoleAsync(user.Id, "Restricted").Result;

            return result;
        }

        public static bool IsInRole(this ApplicationUser user, string role)
        {
            var manager = GetUserManager();

            var result = manager.IsInRoleAsync(user.Id, role).Result;

            return result;
        }

        public static bool IsChatUser(this IPrincipal ident)
        {
            return IsInRole(ident, "ChatUser");
        }

        public static bool IsChatUser(this ApplicationUser user)
        {
            return IsInRole(user, "ChatUser");
        }

        public static ChatUser GetChatUser(this IPrincipal ident)
        {
            var context = new ApplicationDbContext();
            var displayName = ident.DisplayName();
            var user = context.ChatUsers.Include(c => c.Connections).FirstOrDefault(u => u.UserName == displayName);

            return user;
        }

        public static List<ChatUserRole> GetChatUserRoles(this IPrincipal ident)
        {
            using (var context = new ApplicationDbContext())
            {
                var displayName = ident.DisplayName();
                var user = context.ChatUsers.Include(c => c.Connections).FirstOrDefault(u => u.UserName == displayName);
                var roles = context.UserChatRoles.Include(c => c.Role).Where(u => u.User.Id == user.Id).ToList();

                return roles;
            }
        }

        public static bool EnableChatPing(this IPrincipal ident)
        {
            using (var context = new ApplicationDbContext())
            {
                var displayName = ident.DisplayName();
                var user = context.Users.FirstOrDefault(u => u.DisplayName == displayName);

                if (user == null)
                    return true;

                return user.Settings.PingForMessages;
            }
        }

        public static bool UsesMiniChat(this IPrincipal ident)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == ident.Identity.Name);

                return user?.Settings != null && user.Settings.ShowMiniChat;
            }
        }

        private static ApplicationUserManager GetUserManager(ApplicationDbContext context = null)
        {
            if (context == null)
                context = new ApplicationDbContext();
            return new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        private static ApplicationUser GetUser(IPrincipal ident)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Users.FirstOrDefault(u => u.UserName == ident.Identity.Name);
            }
        }
    }
}