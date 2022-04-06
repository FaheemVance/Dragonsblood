using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class BanExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId="", string room ="")
        {
            var name = parameters.Aggregate((fullname, next) => fullname += next);

            if (string.IsNullOrEmpty(name))
            {
                NotifyUser("Please enter a valid name");
                return false;
            }

            using (var context = AppContext)
            {
                var displayName = name;
                var user = context.ChatUsers.Include(c => c.Connections).FirstOrDefault(u => u.UserName == displayName);

                if (user == null)
                    return false;

                user.Banned = true;
                context.SaveChanges();

                NotifyUser($"User {displayName} has been banned");

                foreach (var connection in user.Connections)
                {
                    Hub.Clients.Client(connection.ConnectionId)
                        .sendErrorMessage(
                            "You have been Banned from chat. Please talk to DodgyMaster for more information");
                }
            }

            return true;
        }
    }
}
