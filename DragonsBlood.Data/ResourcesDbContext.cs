using System.Configuration;
using System.Data.Entity;
using DragonsBlood.Data.Allies;
using DragonsBlood.Data.Enemies;
using DragonsBlood.Data.Feedback;
using DragonsBlood.Models.Announcements;
using DragonsBlood.Models.Documents;

namespace DragonsBlood.Data
{
    public class ResourcesDbContext : DbContext
    {
        public ResourcesDbContext() : base(ConfigurationManager.AppSettings["ConnectionPropertyName"])
        {

        }

        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Alliance> Alliances { get; set; }
        public virtual DbSet<Enemy> Enemies { get; set; }
        public virtual DbSet<FeedbackItem> Feedback { get; set; }
    }
}