using System;
using System.Linq;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public static class ChatValidation
    {
        public static bool ValidateSession(ChatUser user, Session session)
        {
            DateTime cutoffDateTime = DateTime.UtcNow - new TimeSpan(6, 0, 0);
            if (session.Connections.Any() && session.CreationStamp > cutoffDateTime)
                return true;

            return false;
        }

        public static bool ValidateConnection(ChatUser user, Connection connection)
        {

            return false;
        }
    }
}
