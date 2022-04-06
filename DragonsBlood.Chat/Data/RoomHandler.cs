using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DragonsBlood.Chat.Hubs;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public class RoomHandler
    {
        private ChatHub Hub { get; set; }
        public RoomHandler(ChatHub hub)
        {
            Hub = hub;
        }

        internal ChatRoom CreateRoom(string roomName, string permissionGroup)
        {
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(permissionGroup))
                return null;

            using (var context = new ApplicationDbContext())
            {
                var sanitisedRoomName = roomName.Replace(" ", "-");
                var displayName = Hub.Context.User.GetChatUser().UserName;
                var existingRoom = context.ChatRooms.FirstOrDefault(r => r.Name == sanitisedRoomName);
                var user = context.ChatUsers.First(c => c.UserName == displayName);

                if (existingRoom != null)
                {
                    AddUserToRoom(sanitisedRoomName, user);
                    return existingRoom;
                }

                var room = new ChatRoom()
                {
                    Name = sanitisedRoomName
                };
                var chatRoomUser = new ChatRoomUsers()
                {
                    Room = room,
                    User = user
                };
                var permission = new ChatRoomPermission();
                if (permissionGroup == "Select a user group...")
                {
                    permission.Room = room;

                    var newRole = new ChatRole()
                    {
                        Name = "Room-" + sanitisedRoomName,
                        Description = "Custom permission for room: " + sanitisedRoomName
                    };

                    permission.Role = newRole;
                }
                else
                {
                    var role = context.ChatRoles.First(r => r.Name == permissionGroup);
                    permission.Role = role;
                    permission.Room = room;
                }

                context.ChatRooms.Add(room);
                context.RoomUsers.Add(chatRoomUser);
                context.RoomPermissions.Add(permission);

                context.SaveChanges();

                UpdateUserAfterRoomJoined(user);

                return room;
            }
        }

        internal void AddUserToRoom(string room, ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var sanitisedRoomName = room.Replace(" ", "-");
                var chatRoom = context.ChatRooms.FirstOrDefault(r => r.Name == sanitisedRoomName);
                var chatRoomUsers = context.RoomUsers.Where(r => r.Room.Name == chatRoom.Name);
                var chatUser = context.ChatUsers.First(u => u.UserName == user.UserName);

                if (chatRoom == null)
                    return;

                if (chatRoomUsers.Any(r => r.User.UserName == user.UserName))
                {
                    return;
                }

                var roomUser = new ChatRoomUsers
                {
                    User = chatUser,
                    Room = chatRoom
                };

                context.RoomUsers.Add(roomUser);
                context.SaveChanges();
            }
            UpdateInitial();
        }

        internal void RemoveFromRoom(string room, ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var sanitisedRoomName = room.Replace(" ", "-");
                var chatRoom = context.ChatRooms.FirstOrDefault(r => r.Name == sanitisedRoomName);
                var chatRoomUsers = context.RoomUsers.Include(r => r.User).Where(r => r.Room.Name == chatRoom.Name);

                if (chatRoom == null)
                    return;

                if (!chatRoomUsers.Any(r => r.User.UserName == user.UserName))
                {
                    return;
                }

                var chatRoomUser = chatRoomUsers.First(r => r.User.UserName == user.UserName);

                context.RoomUsers.Remove(chatRoomUser);
                context.SaveChanges();
            }
            UpdateInitial();
        }

        internal void SetCurrentRoom(string room, ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var chatRoom = context.ChatRooms.FirstOrDefault(r => r.Name == room);

                if (chatRoom == null)
                    return;

                var contextUser = context.ChatUsers.FirstOrDefault(u => u.Id == user.Id);

                if (contextUser == null)
                    return;

                contextUser.CurrentRoom = room;
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext())
            {
                var chatRoom = context.ChatRooms.FirstOrDefault(r => r.Name == room);

                if (chatRoom == null)
                    return;

                var roomUsers = chatRoom.RoomUsers.Where(u => u.User.Connections.Any(c => c.Connected)).Select(u => u.User.UserName);

                Hub.Clients.Caller.updateRoomUsers(roomUsers);
            }
            UpdateMotd(room, user);
        }

        internal void UpdateMotd(string room, ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var chatRoom = context.ChatRooms.FirstOrDefault(r => r.Name == room);

                if (chatRoom == null)
                    return;

                var message = chatRoom.Motd;

                Hub.Clients.Caller.updateMotd(message);
            }
        }

        internal void UpdateInitial()
        {
            using (var context = new ApplicationDbContext())
            {
                var user = Hub.Context.User.GetChatUser();

                if (user == null)
                    return;

                var usersRooms = context.RoomUsers.Where(r => r.User.UserName == user.UserName);
                var rooms = context.ChatRooms.Where(r => usersRooms.Any(u => u.Room == r));
                
                var roomArray = rooms.Select(r => r.Name).ToArray();
                var index = user.CurrentRoom == null ? 0 : Array.IndexOf(roomArray, user.CurrentRoom);

                Hub.Clients.Caller.setRooms(roomArray, index);
                UpdateMotd(user.CurrentRoom, user);
            }
        }

        internal void UpdateInitial(ChatUser chatUser)
        {
            using (var context = new ApplicationDbContext())
            {
                var userRoomPermissions = context.UserChatRoles.Include(c => c.User).Include(c => c.Role).Where(r => r.User.UserName == chatUser.UserName).ToList();
                var roomPermissions = context.RoomPermissions.Include(r=>r.Room).Include(r=>r.Role).ToList();
                var usersRooms = context.RoomUsers.Where(r => r.User.UserName == chatUser.UserName).ToList();
                var rooms = new List<string>();

                foreach (var room in usersRooms)
                {
                    var roomPermission = roomPermissions.First(r => r.Room.Name == room.Room.Name);
                    var userPermission =
                        userRoomPermissions.FirstOrDefault(
                            p => p.User.UserName == chatUser.UserName && p.Role.Name == roomPermission.Role.Name);

                    if (userPermission == null)
                        continue;

                    rooms.Add(room.Room.Name);
                }

                var roomArray = rooms.ToArray();
                foreach (var connection in chatUser.Connections)
                {
                    Hub.Clients.Client(connection.ConnectionId).setRooms(roomArray);
                }
            }
        }

        private void UpdateAllUsersRoomLists()
        {
            using (var context = new ApplicationDbContext())
            {
                var rooms = context.ChatRooms.ToList();


            }
        }

        private void UpdateUserAfterRoomJoined(ChatUser user)
        {
            using (var context = new ApplicationDbContext())
            {
                var usersRooms = context.RoomUsers.Where(r => r.User.UserName == user.UserName);
                var rooms = context.ChatRooms.Where(r => usersRooms.Any(u => u.Room.Name == r.Name));

                var roomArray = rooms.ToList().Select(r => r.Name).ToArray();

                var userConnections =
                    context.ChatUsers.Include(c => c.Connections).First(c => c.UserName == user.UserName).Connections.Where(c => c.Connected).ToList();

                foreach (var connection in userConnections)
                {
                    Hub.Clients.Client(connection.ConnectionId).setRooms(roomArray);
                }
            }
        }
    }
}
