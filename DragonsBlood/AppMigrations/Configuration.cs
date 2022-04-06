using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using DragonsBlood.Data;
using DragonsBlood.Models.ChatModels;
using DragonsBlood.Models.Roles;
using DragonsBlood.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DragonsBlood.AppMigrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"AppMigrations";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var applicationRoles = new List<ApplicationRole>()
            {
                new ApplicationRole("Admin", "Master of all the site"),
                new ApplicationRole("ChatUser", "Allows a user to Chat"),
                new ApplicationRole("Member", "Standard website user"),
                new ApplicationRole("Restricted", "New User or Banned User"),
                new ApplicationRole("Moderator", "Elevated user with basic commands")
            };

            CreateRoles(applicationRoles);

            var faheem = new ApplicationUser()
            {
                UserName = "faheem",
                DisplayName = "HonorlessLegend",
                Email = "faheem@faheemvance.com",
                Level = 17,
                Settings = new UserSettings()
                {
                    ChatNameColor = Color.Yellow.Name,
                    InitialChatMessagesToDisplay = 50,
                    ShowMiniChat = true,
                    PingForMessages = true,
                    PulseForAlerts = true
                }
            };

            var faheemsRoles = new List<string>()
            {
                "Admin",
                "ChatUser"
            };

            CreateUser(faheem, faheemsRoles);

            AddTestAlerts(context);
            AddChatFeatures();
        }
        private void CreateUser(ApplicationUser user, List<string> roles)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var existing = userManager.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (existing == null)
            {
                userManager.Create(user, "Password1");

                foreach (var role in roles)
                {
                    userManager.AddToRole(user.Id, role);
                }

            }
        }
        private void CreateRoles(List<ApplicationRole> roles)
        {
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(new ApplicationDbContext()));

            foreach (var role in roles)
            {
                var r = roleManager.FindByNameAsync(role.Name);

                if (r.Result == null)
                {
                    roleManager.Create(role);
                }
            }
        }
        private void AddTestAlerts(ApplicationDbContext context)
        {/*
            context.Alerts.Add(new Alert()
            {
                Attacker = "Some Douche",
                ShortKingdom = ShortKingdom.High,
                Creator = context.ChatUsers.First(c => c.UserName == "HonorlessLegend"),
                Coordinates = new Coordinates()
                {
                    X = -32,
                    Y = 0
                },
                TimeStamp = DateTime.UtcNow
            });

            context.Alerts.Add(new Alert()
            {
                Attacker = "Some Guy",
                ShortKingdom = ShortKingdom.Ice,
                Creator = context.ChatUsers.First(c => c.UserName == "HonorlessLegend"),
                Coordinates = new Coordinates()
                {
                    X = -60,
                    Y = -32
                },
                TimeStamp = DateTime.UtcNow
            });*/
        }


        private void AddChatFeatures()
        {
            AddDefaultChatRoom();
            AddDefaultChatRoles();
            AddUserToChatRoleForDefaultRoom();
        }
        private void AddDefaultChatRoom()
        {
            using (var context = new ApplicationDbContext())
            {
                var rooms = context.ChatRooms;

                var users = context.ChatUsers;
                var messages = context.ChatMessages;

                if (!rooms.Any(r => r.Name == "All"))
                {
                    var room = new ChatRoom
                    {
                        Name = "All",
                        RoomUsers = new List<ChatRoomUsers>(),
                        Messages = new List<ChatMessage>(),
                        IsDefault = true
                    };

                    foreach (var user in users)
                    {
                        room.RoomUsers.Add(new ChatRoomUsers() { Room = room, User = user });
                    }

                    foreach (var message in messages)
                    {
                        room.Messages.Add(message);
                    }

                    rooms.Add(room);
                    context.SaveChanges();
                }
                else
                {
                    var room = context.ChatRooms.First(r => r.Name == "All");

                    if (!room.IsDefault)
                    {
                        room.IsDefault = true;
                        context.SaveChanges();
                    }
                }
            }
            AddUsersToDefaultChatRoom();
        }
        private void AddUsersToDefaultChatRoom()
        {
            using (var context = new ApplicationDbContext())
            {
                var chatUsers = context.ChatUsers.ToList();
                var roomUsers = context.RoomUsers.ToList();

                var rooms = context.ChatRooms.Include(c => c.RoomUsers).ToList();

                foreach (var room in rooms)
                {
                    if (room.Name != "All")
                        continue;

                    foreach (var user in chatUsers)
                    {
                        if (roomUsers.Any(r => r.Room.Name == room.Name && r.User.UserName == user.UserName))
                            continue;

                        var roomUser = new ChatRoomUsers
                        {
                            User = user,
                            Room = room
                        };

                        context.RoomUsers.Add(roomUser);
                        context.SaveChanges();
                    }
                }
            }
        }
        private void AddDefaultChatRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var existingRoles = context.ChatRoles.ToList();
                var defaultRoles = new List<ChatRole>()
                {
                    new ChatRole() {Name = "Admin", Description = "Manager of all the chat functions", IsDefault = true},
                    new ChatRole() {Name = "Moderator", Description = "Enables a user Moderator abilities", IsDefault = true},
                    new ChatRole() {Name = "User", Description = "Allows a user to use the Chat Box", IsDefault = true},
                    new ChatRole() {Name = "Room-All", Description = "Allows access to the room `All` ", IsDefault = true}
                };
                foreach (var role in defaultRoles)
                {
                    if (existingRoles.Any(e => e.Name == role.Name))
                    {
                        if (!existingRoles.First(e => e.Name == role.Name).IsDefault)
                        {
                            var dbRole = context.ChatRoles.First(r => r.Name == role.Name);
                            dbRole.IsDefault = true;
                            context.SaveChanges();
                        }
                        continue;
                    }

                    context.ChatRoles.Add(role);
                    context.SaveChanges();
                }
            }
        }
        private void AddUserToChatRoleForDefaultRoom()
        {
            using (var context = new ApplicationDbContext())
            {
                var users = context.ChatUsers.ToList();
                var role = context.ChatRoles.First(c => c.Name == "Room-All");

                foreach (var user in users)
                {
                    var userChatRole =
                        context.UserChatRoles.FirstOrDefault(
                            r => r.User.UserName == user.UserName && r.Role.Name == role.Name);

                    if (userChatRole != null)
                        continue;

                    userChatRole = new ChatUserRole
                    {
                        User = user,
                        Role = role
                    };

                    context.UserChatRoles.Add(userChatRole);
                    context.SaveChanges();
                }
            }
        }
    }
}
