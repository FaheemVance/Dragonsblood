using System.Collections.Generic;
using System.Web.Configuration;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class MmExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId = "", string room = "")
        {
            if (parameters.Count > 0)
            {
                NotifyUser("Please use command without parameters: /mm");
                return false;
            }
            var urlAppend = WebConfigurationManager.AppSettings["UrlAppend"];
            if (string.IsNullOrEmpty(requestorConnectionId))
            {
                var user = ChatUser;

                foreach (var connection in user.Connections)
                {
                    Hub.Clients.Client(connection.ConnectionId).redirect($"{urlAppend}/Members");
                }
            }
            else
            {
                Hub.Clients.Client(requestorConnectionId).redirect($"{urlAppend}/Members");
            }

            return false;
        }
    }
}
