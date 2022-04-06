using System;
using System.ComponentModel.DataAnnotations;
using DragonsBlood.Data.Types;

namespace DragonsBlood.Models.Announcements
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        [DataType(DataType.Html)]
        public string Body { get; set; }
        public DateTime Stamp { get; set; }
        public AnnounceType Type {get;set;}
    }
}