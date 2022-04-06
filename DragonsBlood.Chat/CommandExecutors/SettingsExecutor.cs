using System.Collections.Generic;
using System.Web.Configuration;
using DragonsBlood.Data.ResourceMigrations;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class SettingsExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId = "", string room = "")
        {
            if (parameters.Count > 0)
            {
                NotifyUser("Please use the command without parameters: /settings");
                return false;
            }
            var urlAppend = WebConfigurationManager.AppSettings["UrlAppend"];
            if (string.IsNullOrEmpty(requestorConnectionId))
            {
                var user = ChatUser;

                foreach (var connection in user.Connections)
                {
                    Hub.Clients.Client(connection.ConnectionId).redirect($"{urlAppend}/Manage/ChangeSettings");
                }
            }
            else
            {
                Hub.Clients.Client(requestorConnectionId).redirect($"{urlAppend}/Manage/ChangeSettings");
            }

            
            return true;
        }
    }
}
