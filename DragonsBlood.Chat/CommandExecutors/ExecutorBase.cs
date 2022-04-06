using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Principal;
using System.Web;
using DragonsBlood.Chat.Data;
using DragonsBlood.Chat.Hubs;
using DragonsBlood.Chat.Interfaces;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;
using DragonsBlood.Models.Users;
using Microsoft.AspNet.SignalR;

namespace DragonsBlood.Chat.CommandExecutors
{
    public class ExecutorBase : IExecutor
    {
        public ApplicationDbContext AppContext => new ApplicationDbContext();
        public IPrincipal SessionUser => HttpContext.Current.User;
        public ChatUser ChatUser => HttpContext.Current.User.GetChatUser();
        public IHubContext Hub => GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

        public virtual bool Execute(List<string> parameters, string requestorConnectionId = "", string room = "")
        {
            return false;
        }

        internal string GetUserColor(ApplicationUser user)
        {
            if (user.Settings != null)
            {
                if (user.Settings.ChatNameColor != null || user.Settings.ChatNameColor == "0")
                {
                    var color = new Color().GetSystemDrawingColorFromHexString(user.Settings.ChatNameColor);

                    if (color.IsKnownColor)
                        return color.Name;

                    return $"#{color.Name.Remove(0, 2)}";
                }
            }
            return Color.Yellow.Name;
        }

        internal void NotifyUser(string message)
        {
            var user = ChatUser;

            var chatMessage = new ChatMessage()
            {
                User ="System",
                Message = message,
                Edited = false,
                EditedBy = string.Empty,
                Removed = false,
                TimeStamp = DateTime.UtcNow
            };

            foreach (var connection in user.Connections)
            {
                Hub.Clients.Client(connection.ConnectionId)
                    .sendMessage(MessageHandler.FormatMessage(chatMessage, Color.Red.Name), false, "All");
            }
        }
    }
}
