using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.Models.Roles
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string name, string description)
            : base(name)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}