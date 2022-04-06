using System;
using System.Collections.Generic;
using System.Linq;
using DragonsBlood.Chat.Data;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat
{
    public static class CommandHandler
    {
        public static bool IsCommand(string message, List<ChatUserRole> roles)
        {
            if (message.Length < 2)
                return false;

            if (message.Remove(1).Contains("/"))
            {
                return GetCommand(message, roles) != null;
            }

            return false;
        }

        private static ChatCommand GetCommand(string message, List<ChatUserRole> roles)
        {
            message = message.Remove(0, 1);
            var sectioned = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var command = Commands.VerifyCommand(sectioned, roles);

            return command;
        }

        public static bool HandleCommand(string message, List<ChatUserRole> roles, int userId, string connectionId, string room)
        {
            var command = GetCommand(message, roles);

            return command != null && CommandExecutor.ExecuteCommand(command, connectionId, room);
        }
    }
}
