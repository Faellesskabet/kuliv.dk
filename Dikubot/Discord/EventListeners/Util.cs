using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners;

public class Util
{
    /// <Summary>Checks if the first entry in the audit log is a bot.</Summary>
    /// <param name="guild">The guild that is being checked for.</param>
    /// <return>Task<bool>.</return>
    public async Task<bool> IsBotFirstEntryInAuditLog(SocketGuild guild)
    {
        await foreach (IReadOnlyCollection<RestAuditLogEntry> auditLogEntryCollection in guild.GetAuditLogsAsync(1))
        foreach (RestAuditLogEntry auditLogEntry in auditLogEntryCollection)
            return auditLogEntry.User.IsBot;
        return false;
    }

    public static bool HasRole(SocketUser user, SocketGuild guild, string name)
    {
        if (guild == null) throw new ArgumentException("Guild may not be null");
        SocketRole role = guild.Roles.ToList().Find(socketRole => socketRole.Name == name);
        return role != null && guild.GetUser(user.Id).Roles.Contains(role);
    }

    public static bool IsMod(SocketUser user, SocketGuild guild)
    {
        return HasRole(user, guild, "Mod");
    }

    public static void GrantRole(SocketGuildUser user, SocketGuild guild, string name)
    {
        user.AddRoleAsync(guild.Roles.FirstOrDefault(role => role.Name == name));
    }

    public static void RemoveRole(SocketGuildUser user, SocketGuild guild, string name)
    {
        user.RemoveRoleAsync(guild.Roles.FirstOrDefault(role => role.Name == name));
    }
}