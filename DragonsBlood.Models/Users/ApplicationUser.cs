using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using DragonsBlood.Models.AlertModels;
using DragonsBlood.Models.ChatModels;
using DragonsBlood.Models.Roles;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.Models.Users
{
    public class ApplicationUser : IdentityUser
    {

        [MaxLength(200)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public int Level { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ICollection<Alert> Alerts { get; set; }

        public virtual UserSettings Settings { get; set; }

        public virtual ChatUser ChatUser { get; set; }

        public ICollection<ApplicationRole> AppRoles { get; set; }
    }
}