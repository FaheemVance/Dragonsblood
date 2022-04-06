using System;

namespace DragonsBlood.Models.ChatModels
{
    public class Connection
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public bool Connected { get; set; }
        public string UserAgent { get; set; }
        public DateTime ConnectionTime { get; set; }
    }
}