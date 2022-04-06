using System.Collections.Generic;
using System.Linq;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class LevelExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId="", string room="")
        {
            using (var context = AppContext)
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == SessionUser.Identity.Name);

                if (user == null)
                    return false;

                user.Level = int.Parse(parameters[0]);
                context.SaveChanges();

                NotifyUser($"Your level has been updated. Your new level is {user.Level}");

                return true;
            }
        }
    }
}
