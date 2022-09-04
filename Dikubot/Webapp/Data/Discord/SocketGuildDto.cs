using Discord.WebSocket;

namespace Data.Discord;

/// <summary>
/// It annoys me that this is the only workaround I could find
/// </summary>
public class SocketGuildDto
{
    public SocketGuildDto(SocketGuild guild)
    {
        Guild = guild;
    }

    public SocketGuild Guild { get; }
}