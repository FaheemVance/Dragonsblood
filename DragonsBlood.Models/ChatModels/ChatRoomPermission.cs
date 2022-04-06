namespace DragonsBlood.Models.ChatModels
{
    public class ChatRoomPermission
    {
        public int Id { get; set; }
        public virtual ChatRoom Room { get; set; }
        public virtual ChatRole Role { get; set; }
    }
}
