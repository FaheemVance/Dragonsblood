using System.Collections.Generic;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; } 
        public virtual ICollection<ChatRoomUsers> RoomUsers { get; set; } 
        public bool IsDefault { get; set; }
        public string Motd { get; set; }
    }
}
