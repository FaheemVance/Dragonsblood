using System;
using System.Data.Entity.Migrations;
using System.Linq;
using DragonsBlood.Data.Types;
using DragonsBlood.Models.Announcements;
using DragonsBlood.Models.Documents;

namespace DragonsBlood.Data.ResourceMigrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ResourcesDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"ResourceMigrations";
        }

        protected override void Seed(ResourcesDbContext context)
        {
            /*
            CreateAnnouncement(context, "New Website", @"We have a new website <br /> Welcome to our new website, please check back often for updates and information.", AnnounceType.News);
            CreateAnnouncement(context, "Planned Attack", "<p> Mass attack against \"House of Lannister\" (Supporting \"Wolfen\" Clans) <br><b>8PM Sunday 20th March</b> </p>", AnnounceType.Attack);
            CreateDocument(context, "Welcome", "Library/Welcome.pdf");
            CreateDocument(context, "Moving Your Castle", "Library/Moving your Castle.pdf");
            */
        }

        private void CreateAnnouncement(ResourcesDbContext context, string subject, string body, AnnounceType type)
        {
            if (!context.Announcements.Any(a => a.Subject == subject && a.Body == body))
                context.Announcements.Add(new Announcement() { Subject = subject, Body = body, Stamp = DateTime.Now, Type = type});
        }

        private void CreateDocument(ResourcesDbContext context, string docName, string docPath)
        {
            if (!context.Documents.Any(d => d.DisplayText == docName && d.Location == docPath))
                context.Documents.Add(new Document() { DisplayText = docName, Location = docPath });
        }
    }
}
