using System.Collections.Generic;

namespace DragonsBlood.Models.ChatModels
{
    public class ChatCommand
    {
        public int Id { get; set; }
        public CommandType Type { get; set; }
        public List<string> Parameters { get; set; }
    }
}
