using System.Collections.Generic;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatRoomPermissionModel
    {
        public List<string> Rooms { get; set; } 
        public List<string> Permissions { get; set; }
        
        public string SelectedRoom { get; set; } 
        public string SelectedPermission { get; set; }
    }
}
