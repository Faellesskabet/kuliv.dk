using System.Threading.Tasks;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners
{
    public class Util
    {
        /// <Summary>Checks if the first entry in the audit log is a bot.</Summary>
        /// <param name="guild">The guild that is being checked for.</param>
        /// <return>Task<bool>.</return>
        public async Task<bool> IsBotFirstEntryInAuditLog(SocketGuild guild)
        {
            await foreach (var auditLogEntryCollection in guild.GetAuditLogsAsync(1))
            foreach (var auditLogEntry in auditLogEntryCollection)
                return auditLogEntry.User.IsBot;
            return false;
        }
    }
}