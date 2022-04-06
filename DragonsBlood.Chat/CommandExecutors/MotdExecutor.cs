using System.Collections.Generic;
using System.Linq;

namespace DragonsBlood.Chat.CommandExecutors
{
    public class MotdExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId = "", string room = "")
        {
            using (var context = AppContext)
            {
                if (room.Contains(" "))
                    room = room.Replace(" ", "-");

                var foundRoom = context.ChatRooms.FirstOrDefault(r => r.Name == room);

                if (foundRoom == null)
                    return false;

                foundRoom.Motd = string.Join(" ", parameters);
                context.SaveChanges();
                Hub.Clients.Client(requestorConnectionId).updateMotd(foundRoom.Motd);
                return true;
            }
        }
    }
}
