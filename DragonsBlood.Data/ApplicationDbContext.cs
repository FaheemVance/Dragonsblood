using System.Configuration;
using System.Data.Entity;
using DragonsBlood.Models.AlertModels;
using DragonsBlood.Models.ChatModels;
using Microsoft.AspNet.Identity.EntityFramework;
using DragonsBlood.Models.Users;

namespace DragonsBlood.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConfigurationManager.AppSettings["ConnectionPropertyName"])
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatUser> ChatUsers { get; set; }
        public virtual DbSet<ChatRoom> ChatRooms { get; set; } 
        public virtual DbSet<ChatRoomUsers> RoomUsers { get; set; }
        public virtual DbSet<ChatRoomPermission> RoomPermissions { get; set; } 
        public virtual DbSet<Connection> Connections { get; set; } 
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<ChatRole> ChatRoles { get; set; }
        public virtual DbSet<ChatUserRole> UserChatRoles { get; set; } 
        public virtual DbSet<ChatMessageArchive> MessageArchive { get; set; }
    }
}