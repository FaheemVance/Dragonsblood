using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public static class CommandExecutor
    {
        public static bool ExecuteCommand(ChatCommand command, string connectionId, string room)
        {
            var executor = ExecuteHelper.GetExecutor(command.Type);
            return executor.Execute(command.Parameters, connectionId, room);
        }
    }
}
