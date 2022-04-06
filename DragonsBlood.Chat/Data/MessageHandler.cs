using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using DragonsBlood.Chat.Hubs;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public class MessageHandler
    {
        private ChatHub Hub { get; }
        public MessageHandler(ChatHub hub)
        {
            Hub = hub;
        }

        public void HandleMessage(string message, string room)
        {
            var type = GetMessageType(message);

            switch (type)
            {
                case MessageType.Standard:
                    SendStandardMessage(message, room);
                    break;
                case MessageType.Command:
                    SendCommand(message, room);
                    break;
                case MessageType.Group:
                    break;
                case MessageType.User:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void PopulateMessagesForUser()
        {
            using (var context = new ApplicationDbContext())
            {
                var userSettings = context.Users.FirstOrDefault(u => u.UserName == Hub.Context.User.Identity.Name)?.Settings;

                if (userSettings == null)
                    return;

                var user = Hub.Context.User.GetChatUser();

                var usersRooms = context.RoomUsers.Where(r => r.User.UserName == user.UserName);

                var rooms = context.ChatRooms.Include(r => r.Messages).Where(r => usersRooms.Any(u => u.Room == r)).ToList();

                foreach (var room in rooms)
                {
                    var messages =
                        room.Messages.OrderByDescending(c => c.TimeStamp)
                            .Take(userSettings.InitialChatMessagesToDisplay)
                            .ToList()
                            .OrderBy(c => c.TimeStamp)
                            .ToList();
                    
                    Hub.Clients.Caller.updateRoomMessages(FormatMessageList(messages).ToArray(), room.Name);
                }
            }
        }

        private MessageType GetMessageType(string message)
        {
            if (message.StartsWith("/"))
                return MessageType.Command;

            if (message.StartsWith("@"))
                return MessageType.User;

            if (message.Contains("#"))
                return MessageType.Group;

            return MessageType.Standard;
        }

        private void SendStandardMessage(string message, string room)
        {
            using (var context = new ApplicationDbContext())
            {
                var roomSanitised = room.Replace(" ", "-");
                var chatMessage = SaveNewMessage(message, Hub.Context.User.GetChatUser(), roomSanitised);

                var chatRoom = context.ChatRooms.First(r => r.Name == roomSanitised);

                if (chatRoom == null)
                    return;

                var usersInRoom = context.RoomUsers.Where(r => r.Room.Name == chatRoom.Name).Select(s => s.User.UserName).ToList();

                var users = context.ChatUsers.Include(c => c.Connections).Where(c => usersInRoom.Contains(c.UserName)).ToList();

                foreach (var user in users)
                {
                    var doPing = PingUser(user);
                    foreach (var connection in user.Connections)
                    {
                        Hub.Clients.Client(connection.ConnectionId).sendMessage(FormatMessage(chatMessage), doPing, roomSanitised);
                    }
                }
            }
        }

        private void SendCommand(string message, string room)
        {
            var roles = Hub.Context.User.GetChatUserRoles();

            if (CommandHandler.IsCommand(message, roles))
            {
                CommandHandler.HandleCommand(message, roles, Hub.Context.User.GetChatUser().Id, Hub.Context.ConnectionId, room);
            }
        }

        private ChatMessage SaveNewMessage(string message, ChatUser user, string room)
        {
            using (var context = new ApplicationDbContext())
            {
                var chatRoom = context.ChatRooms.Include(c => c.Messages).First(r => r.Name == room);


                if (chatRoom == null)
                    return null;

                var chatMessage = new ChatMessage()
                {
                    User = user.UserName,
                    Message = message,
                    TimeStamp = DateTime.UtcNow
                };
                context.ChatMessages.Add(chatMessage);
                chatRoom.Messages.Add(chatMessage);
                context.SaveChanges();

                return chatMessage;
            }
        }

        private static string GetUserColor(string userName)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.DisplayName == userName);

                if (user == null)
                    return Color.Yellow.Name;

                if (user.Settings != null)
                {
                    return GetUserColorFromColor(user.Settings.ChatNameColor);
                }

                return Color.Yellow.Name;
            }
        }

        private static string GetUserColorFromColor(string color)
        {
            if (!string.IsNullOrEmpty(color) && color != "0")
            {
                var parsedColor = new Color().GetSystemDrawingColorFromHexString(color);

                if (parsedColor.IsKnownColor)
                    return parsedColor.Name;

                return $"#{parsedColor.Name.Remove(0, 2)}";
            }

            return Color.Yellow.Name;
        }

        private bool PingUser(ChatUser displayUser)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.DisplayName == displayUser.UserName);

                if (user?.Settings == null)
                    return true;

                return user.Settings.PingForMessages;
            }
        }

        private List<string> FormatMessageList(List<ChatMessage> messages)
        {
            return (from message in messages
                    let color = GetUserColor(message.User)
                    select
                        $"<li class=\"new\"><strong class=\"tooltip\" style=\"color: {color}\"><span class=\"tooltiptext\">{message.TimeStamp.ToString("dd-MMM-yy HH:mm")}</span>{message.User}:&nbsp;</strong>{message.Message}</li>")
                .ToList();
        }

        public static string FormatMessage(ChatMessage message, string color = null)
        {
            if(string.IsNullOrEmpty(color))
                color = GetUserColor(message.User);
            return
                $"<li class=\"new\"><strong class=\"tooltip\" style=\"color: {color}\"><span class=\"tooltiptext\">{message.TimeStamp.ToString("dd-MMM-yy HH:mm")}</span>{message.User}:&nbsp;</strong>{message.Message}</li>";

        }
    }
}
