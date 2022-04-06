using System;
using System.Collections.Generic;
using System.Linq;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public static class Commands
    {
        public static List<ChatCommand> GetUserCommands()
        {
            return new List<ChatCommand>
            {
                new ChatCommand
                {
                    Type = CommandType.Alert,
                    Parameters = new List<string>(2)
                    {
                        "{kingdom}",
                        "{attacker}",
                        "{location}"
                    }
                },
                new ChatCommand
                {
                    Type = CommandType.LvlUp,
                    Parameters = new List<string>(0)
                },
                new ChatCommand
                {
                    Type = CommandType.Lvl,
                    Parameters = new List<string>(1)
                    {
                        "{level}"
                    }
                },
                new ChatCommand
                {
                    Type = CommandType.Settings,
                    Parameters = new List<string>(0)
                }
            };
        }

        public static List<ChatCommand> GetAdminCommands()
        {
            return new List<ChatCommand>
            {
                new ChatCommand
                {
                    Type = CommandType.Mm,
                    Parameters = new List<string>(0)
                }
            };
        }

        public static List<ChatCommand> GetModCommands()
        {
            return new List<ChatCommand>
            {
                new ChatCommand
                {
                    Type = CommandType.Ban,
                    Parameters = new List<string>(1)
                    {
                        "{username}"
                    }
                },
                new ChatCommand
                {
                    Type = CommandType.UnBan,
                    Parameters = new List<string>(1)
                    {
                        "{username}"
                    }
                },
                new ChatCommand
                {
                    Type = CommandType.Motd,
                    Parameters = new List<string>()
                }
            };
        }

        public static ChatCommand VerifyCommand(List<string> command, List<ChatUserRole> roles)
        {
            List<ChatCommand> commands = GetUserCommands();

            if (roles.Any(r => r.IsMod()))
                commands = commands.Concat(GetModCommands()).ToList();

            if (roles.Any(r => r.IsAdmin()))
                commands = commands.Concat(GetAdminCommands()).ToList();

            CommandType commandType;
            var success = Enum.TryParse(command.First(), true, out commandType);

            if (!success)
                return null;

            command.RemoveAt(0); // Get rid of the Command leaving the Parameters

            var foundCommand = commands.FirstOrDefault(c => c.Type == commandType);

            if (foundCommand == null)
                return null;

            var chatCommand = new ChatCommand {Type = commandType, Parameters = command};

            return chatCommand;
        }
    }
}
