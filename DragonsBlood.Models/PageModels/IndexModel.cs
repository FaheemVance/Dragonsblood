using System.Collections.Generic;
using DragonsBlood.Models.AlertModels;
using DragonsBlood.Models.Announcements;
using DragonsBlood.Models.Documents;

namespace DragonsBlood.Models.PageModels
{
    public class IndexModel
    {
        public Announcement LatestAnnouncement { get; set; }
        public List<Document> Documents { get; set; }
        public List<Alert> Alerts { get; set; }
    }
}
