namespace DragonsBlood.Models.ChatModels
{
    public class ChatRoomUsers
    {
        public int Id { get; set; }
        public virtual ChatUser User { get; set; } 
        public virtual ChatRoom Room { get; set; }
    }
}
