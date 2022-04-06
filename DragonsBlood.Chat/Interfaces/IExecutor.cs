using System.Collections.Generic;

namespace DragonsBlood.Chat.Interfaces
{
    public interface IExecutor
    {
        bool Execute(List<string> parameters, string requestorConnectionId="", string room = "");
    }
}
