using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DragonsBlood.Data;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Chat.Data
{
    public static class ConnectionHandler
    {
        public static void ClearOldUserConnections()
        {
            using (var context = new ApplicationDbContext())
            {
                var cutOff = DateTime.UtcNow - new TimeSpan(6, 0, 0);

                var orphaned = GetOrphanedConnections();
                var connectionsToRemove =
                    context.Connections.Where(
                        c =>
                            !c.Connected || c.ConnectionTime < cutOff).ToList();

                foreach (var connection in connectionsToRemove.Concat(orphaned.Where(o => connectionsToRemove.Any(c => c.ConnectionId != o.ConnectionId))))
                {
                    if (context.Connections.Any(c => c.ConnectionId == connection.ConnectionId))
                    {
                        context.Connections.Remove(connection);
                        context.SaveChanges();
                    }
                }
            }
        }

        private static List<Connection> GetOrphanedConnections()
        {
            using (var context = new ApplicationDbContext())
            {
                var users = context.ChatUsers.Include(c => c.Connections).ToList();
                var connectionList = context.Connections.ToList();

                foreach (var user in users)
                {
                    foreach (var connection in user.Connections)
                        connectionList.Remove(connection);
                }

                return connectionList;
            }
        }
    }
}
