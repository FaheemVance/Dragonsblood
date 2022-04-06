using System.Collections.Generic;
using System.Linq;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class UnBanExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId="", string room="")
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
                var user = context.ChatUsers.FirstOrDefault(u => u.UserName == displayName);

                if (user == null)
                    return false;

                user.Banned = false;
                context.SaveChanges();

                NotifyUser($"User {displayName} has been un-banned");
            }

            return true;
        }
    }
}
