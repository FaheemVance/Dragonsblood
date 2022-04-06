using System.Collections.Generic;
namespace DragonsBlood.Models.ChatModels
{
    public class ChatIndexModel
    {
        public List<ChatRole> Roles { get; set; }
        public List<ChatRoom> Rooms { get; set; }
        public List<ChatRoomPermission> RoomPermissions { get; set; }   
        public List<ChatUserRole> UserRoles { get; set; } 
    }
}
