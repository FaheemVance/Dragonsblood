using System;
using System.ComponentModel.DataAnnotations;
using DragonsBlood.Models.CustomModels;

namespace DragonsBlood.Models.AlertModels
{
    public class Alert
    {
        public int Id { get; set; }

        [Required]
        public string Attacker { get; set; }
        public Kingdoms Kingdom { get; set; }
        [Display(Name = "Kingdom")]

        [Required]
        public ShortKingdom ShortKingdom { get; set; }

        [Required]
        public Coordinates Coordinates { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Retaliated { get; set; }
        public string CompletedBy { get; set; }
    }
}
