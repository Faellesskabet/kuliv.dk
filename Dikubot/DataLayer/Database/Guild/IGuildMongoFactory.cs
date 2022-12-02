using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild;

public interface IGuildMongoFactory
{
    public T Get<T>(SocketGuild guild);
}