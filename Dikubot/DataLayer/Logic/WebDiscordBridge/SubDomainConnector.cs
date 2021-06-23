using System;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Logic.WebDiscordBridge
{
    public class SubDomainConnector
    {
        public enum Subdomains : ulong
        { 
            lukitest = 786946524310929419,
            discord = 589752263791083541,
            science = 808777327881224273
          
        }
        public static ulong DomainToDiscordId(string domain)
        {
            return SubDomainToDiscordId(domain.Split(".")?[0]);
        }
        
        public static ulong SubDomainToDiscordId(string subdomain)
        {
            subdomain = subdomain.Replace("https://", "");
            if (ulong.TryParse(subdomain, out var id))
            {
                return id;
            }

            try
            {
                return (ulong) Enum.Parse<Subdomains>(subdomain, true);
            }
            catch (Exception e)
            {
                //ignored
            }

            return 0;

        }

        public static SocketGuild GetGuildFromDomain(string domain)
        {
            return DiscordBot.client.GetGuild(DomainToDiscordId(domain));
        }
        
    }
}