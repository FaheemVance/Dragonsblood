using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DragonsBlood.Chat.Data;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;
using Microsoft.AspNet.SignalR;

namespace DragonsBlood.Chat.Hubs
{
    public class ChatHub : Hub
    {
        /* Event Handlers */
        
        public override Task OnConnected()
        {
            var displayName = Context.User.DisplayName();
            ConnectionHandler.ClearOldUserConnections();
            using (var context = new ApplicationDbContext())
            {
                var chatUser =
                    context.ChatUsers.Include(c => c.Connections).FirstOrDefault(u => u.UserName == displayName);

                if (chatUser == null)
                {
                    if (Context.User.IsChatUser() && Context.User.GetChatUser() == null)
                    {
                        UserHandler.CreateChatUser(this, displayName);
                    }
                }
                else
                {
                    UserHandler.AddConnectionToUser(this, chatUser);
                }
            }
            if (Context.User.Identity.IsAuthenticated)
            {
                SetRoomData();

                var handler = new RoomHandler(this);
                handler.UpdateInitial();

                RefreshActiveUsers();
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            using (var context = new ApplicationDbContext())
            {
                var displayName = Context.User.DisplayName();
                var user = context.ChatUsers.Include(c => c.Connections).FirstOrDefault(u => u.UserName == displayName);

                if (user == null)
                    return null;

                if (user.Connections.Any(c => c.ConnectionId == Context.ConnectionId))
                    context.Connections.First(c => c.ConnectionId == Context.ConnectionId).Connected = false;

                context.SaveChanges();

                RefreshActiveUsers();

                return base.OnDisconnected(stopCalled);
            }

        }

        /* Client Handlers */

        public void Send(string message, string room)
        {
            if (room.Contains("-"))
                room = room.Replace("-", " ");

            var handler = new MessageHandler(this);
            handler.HandleMessage(message, room);
            RefreshActiveUsers();
        }

        public void CreateRoom(string name, string permissionGroup)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(permissionGroup))
                return;
            var handler= new RoomHandler(this);
            handler.CreateRoom(name, permissionGroup);
            SetRoomData();
        }

        public void JoinRoom(string name)
        {
            if (string.IsNullOrEmpty(name) || name == "No Rooms Available" || name == "Select a room...")
                return;
            var handler = new RoomHandler(this);
            handler.AddUserToRoom(name, Context.User.GetChatUser());
        }

        public void LeaveRoom(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            var handler = new RoomHandler(this);
            handler.RemoveFromRoom(name, Context.User.GetChatUser());
        }

        public void UpdateRoomList()
        {
            SetRoomData();
        }

        public void GetUpdatedMessages()
        {
            var handler = new MessageHandler(this);
            handler.PopulateMessagesForUser();
        }

        public void GetAddToGroupData(string userName)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.ChatUsers.FirstOrDefault(c => c.UserName == userName);

                if (user == null)
                {
                    Clients.Caller.updateAddToGroup(null, null, null);
                    return;
                }

                var roles = context.UserChatRoles.Where(s => s.User.UserName == user.UserName).Select(s => s.Role.Name).ToList();
                var allRoles = context.ChatRoles.ToList();

                var rolesCanBeAdded = new List<string>();

                foreach (var role in allRoles)
                {
                    if (roles.Any(a => a == role.Name))
                        continue;

                    rolesCanBeAdded.Add(role.Name);
                }

                Clients.Caller.updateAddToGroup(roles.ToArray(), rolesCanBeAdded.ToArray(), user.UserName);
            }
        }

        public void AddUserGroup(string group, string name)
        {
            if (!Context.User.IsAdmin())
                return;

            using (var context = new ApplicationDbContext())
            {
                var user = context.ChatUsers.FirstOrDefault(a => a.UserName == name);

                if (user == null)
                    return;

                var role = context.ChatRoles.FirstOrDefault(r => r.Name == group);
                var userChatRole = new ChatUserRole
                {
                    User = user,
                    Role = role
                };


                context.UserChatRoles.Add(userChatRole);
                context.SaveChanges();
            }
        }

        public void RemoveUserFromGroup(string group, string name)
        {
            if (!Context.User.IsAdmin())
                return;

            using (var context = new ApplicationDbContext())
            {
                var user = context.ChatUsers.FirstOrDefault(a => a.UserName == name);

                if (user == null)
                    return;

                var role = context.ChatRoles.First(r => r.Name == group);
                var userChatRole =
                    context.UserChatRoles.FirstOrDefault(r => r.User.UserName == user.UserName && r.Role.Name == group);

                if (userChatRole == null)
                    return;

                context.UserChatRoles.Remove(userChatRole);
                context.SaveChanges();

                var handler = new RoomHandler(this);
                handler.UpdateInitial(user);
            }
        }

        public void UpdateCurrentRoom(string room)
        {
            if (room.Contains(" "))
                room = room.Replace(" ", "-");

            var handler = new RoomHandler(this);
            handler.SetCurrentRoom(room, Context.User.GetChatUser());
            RefreshActiveUsers();
        }

        public void UpdateMessageOfTheDay(string room)
        {
            if (room.Contains(" "))
                room = room.Replace(" ", "-");

            var handler = new RoomHandler(this);
            handler.UpdateMotd(room, Context.User.GetChatUser());
        }

        /* Sound Settings */

        public void UpdatePing(bool enabled)
        {
            using (var context = new ApplicationDbContext())
            {
                var displayName = Context.User.DisplayName();
                var user = context.Users.FirstOrDefault(u => u.DisplayName == displayName);

                if (user?.Settings == null)
                    return;

                user.Settings.PingForMessages = enabled;
                context.SaveChanges();
            }
        }

        public void UpdatePulse(bool enabled)
        {
            using (var context = new ApplicationDbContext())
            {
                var displayName = Context.User.DisplayName();
                var user = context.Users.FirstOrDefault(u => u.DisplayName == displayName);

                if (user?.Settings == null)
                    return;

                user.Settings.PulseForAlerts = enabled;
                context.SaveChanges();
            }
        }

        /* Client Side Updates */

        private void RefreshActiveUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var users = context.ChatUsers.Include(c => c.Connections)
                    .Where(u => u.Connections.Any(c => c.Connected)).ToList();

                var activeUserNames = users.Select(u => u.UserName).Distinct().ToList();

                foreach (var user in users)
                {
                    foreach (var connection in user.Connections)
                    {
                        Clients.Client(connection.ConnectionId).updateUsers(activeUserNames.ToArray());
                        foreach (var room in context.ChatRooms.Include(r => r.RoomUsers).Where(c => c.Name == user.CurrentRoom))
                        {
                            var list =
                                users.Where(u => u.CurrentRoom == room.Name && u.Connections.Any(c => c.Connected))
                                    .Select(u => u.UserName).ToList();
                            Clients.Client(connection.ConnectionId).updateRoomUsers(list);
                        }
                    }
                }
            }
        }

        private void SetRoomData()
        {
            using (var context = new ApplicationDbContext())
            {
                var user = Context.User.GetChatUser();
                var rooms = context.ChatRooms.Where(r => r.Name != "All").Select(r => r.Name).ToList();
                var permissions = context.RoomPermissions.Include(p => p.Room).Include(p => p.Role).ToList();
                var userPermissions = context.UserChatRoles.Where(u => u.User.UserName == user.UserName).Select(s => s.Role.Name).ToList();
                var usersRoomsJoined = context.RoomUsers.Where(r => r.User.UserName == user.UserName);
                List<string> roles = new List<string>();

                var usersRooms = (from room in rooms
                    let permission = permissions.FirstOrDefault(r => r.Room.Name == room)
                    where permission != null
                    where userPermissions.Any(a => a == permission.Role.Name) || Context.User.IsAdmin()
                    where !usersRoomsJoined.Any(a => a.Room.Name == room)
                    select room).ToList();

                if (!usersRooms.Any())
                    usersRooms.Add("No Rooms Available");

                if (Context.User.IsAdmin())
                {
                    roles = context.ChatRoles.Select(s => s.Name).ToList();
                    Clients.Caller.SetChatRoomData(usersRooms.ToArray(), roles.ToArray());
                    return;
                }

                Clients.Caller.SetChatRoomData(usersRooms.ToArray());
            }
        }

        private void UpdateUsersChatRooms()
        {
            
        }
    }
}