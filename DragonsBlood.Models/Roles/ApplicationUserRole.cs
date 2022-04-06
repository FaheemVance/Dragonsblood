using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.Models.Roles
{
    public class ApplicationUserRole : IdentityUserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ApplicationRole Role { get; set; }
    }
}