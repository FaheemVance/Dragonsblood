using System.ComponentModel.DataAnnotations;

namespace DragonsBlood.Models
{
    public class PublicProfile
    {
        [Display(Name = "Character Name")]
        public string UserName { get; set; }
        public int Level { get; set; }
    }
}