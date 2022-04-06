namespace DragonsBlood.Models.ChatModels
{
    public class ChatUserRole
    {
        public int Id { get; set; }
        public virtual ChatRole Role { get; set; }
        public virtual ChatUser User { get; set; }
    }
}