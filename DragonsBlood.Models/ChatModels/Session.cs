using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragonsBlood.Models.ChatModels
{
    public class Session
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreationStamp { get; set; }
        public virtual ICollection<Connection> Connections { get; set; }

        public int ChatUserId { get; set; }

        [ForeignKey("ChatUserId")]
        public ChatUser ChatUser { get; set; }
    }
}
