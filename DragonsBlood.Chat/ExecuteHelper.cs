using DragonsBlood.Chat.CommandExecutors;
using DragonsBlood.Chat.Interfaces;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat
{
    public static class ExecuteHelper
    {
        public static IExecutor GetExecutor(CommandType type)
        {
            switch (type)
            {
                case CommandType.Alert:
                    return new AlertExecutor();
                case CommandType.Ban:
                    return new BanExecutor();
                case CommandType.UnBan:
                    return new UnBanExecutor();
                case CommandType.Lvl:
                    return new LevelExecutor();
                case CommandType.LvlUp:
                    return new LevelUpExecutor();
                case CommandType.Mm:
                    return new MmExecutor();
                case CommandType.Motd:
                    return new MotdExecutor();
                case CommandType.Settings:
                    return new SettingsExecutor();
                default:
                    return null;
            }
        }
    }
}
