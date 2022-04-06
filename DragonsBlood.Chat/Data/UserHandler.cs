using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DragonsBlood.Chat.Hubs;
using DragonsBlood.Data;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public static class UserHandler
    {
        public static void CreateChatUser(ChatHub hub, string displayName)
        {
            using (var context = new ApplicationDbContext())
            {
                var newUser = new ChatUser
                {
                    UserName = displayName,
                    Connections = new List<Connection>
                            {
                                new Connection
                                {
                                    ConnectionId = hub.Context.ConnectionId,
                                    Connected = true,
                                    UserAgent = hub.Context.Request.Headers["User-Agent"],
                                    ConnectionTime = DateTime.UtcNow
                                }
                            }
                };
                context.ChatUsers.Add(newUser);

                var allRoomRole = context.ChatRoles.FirstOrDefault(c => c.Name == "Room-All");
                var userRoomsRole = context.ChatRoles.FirstOrDefault(c => c.Name == "User");

                if (allRoomRole != null && userRoomsRole != null)
                {
                    var allUserRole = new ChatUserRole()
                    {
                        Role = allRoomRole, User = newUser
                    };
                    var userRoomRole = new ChatUserRole()
                    {
                        Role = userRoomsRole,
                        User = newUser
                    };

                    context.UserChatRoles.Add(allUserRole);
                    context.UserChatRoles.Add(userRoomRole);
                }

                var defaultRoom = context.ChatRooms.FirstOrDefault(r => r.Name == "All");

                if (defaultRoom != null)
                {
                    var usersChatRoom = new ChatRoomUsers
                    {
                        User = newUser,
                        Room = defaultRoom
                    };

                    context.RoomUsers.Add(usersChatRoom);
                }
                context.SaveChanges();
            }
        }

        public static void AddConnectionToUser(ChatHub hub, ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var existingUser =
                    context.ChatUsers.Include(c => c.Connections).FirstOrDefault(c => c.UserName == user.UserName);

                if (existingUser == null)
                    return;

                if (existingUser.Connections == null)
                    existingUser.Connections = new List<Connection>();

                existingUser.Connections.Add(new Connection()
                {
                    ConnectionId = hub.Context.ConnectionId,
                    Connected = true,
                    UserAgent = hub.Context.Request.Headers["User-Agent"],
                    ConnectionTime = DateTime.UtcNow
                });
                context.SaveChanges();
            }
        }
    }
}
