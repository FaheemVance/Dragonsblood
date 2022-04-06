using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool Banned { get; set; }
        public string CurrentRoom { get; set; }
        public virtual ICollection<Connection> Connections { get; set; } 
        public virtual ICollection<ChatRoomUsers> ChatRooms { get; set; } 
    }
}