using System;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Removed { get; set; }
        public bool Edited { get; set; }
        public string EditedBy { get; set; }

        public ChatMessage()
        {

        }

        public ChatMessage(string displayName, string message)
        {
            User = displayName;
            Message = message;
            TimeStamp = DateTime.UtcNow;
        }
    }
}