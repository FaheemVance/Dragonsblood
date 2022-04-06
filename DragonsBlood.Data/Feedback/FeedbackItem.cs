using System;

namespace DragonsBlood.Data.Feedback
{
    public class FeedbackItem
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
