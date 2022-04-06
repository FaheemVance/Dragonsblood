using System;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatMessageArchive
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public string RoomName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
