using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using Dikubot.Database.Models.VoiceChannel;
using Dikubot.Discord;
using Discord;
using MongoDB.Driver;

namespace Dikubot.Database.Models.TextChannel
{
    /// <summary>
    /// Class for for retrieving information from the Channel collection.
    /// </summary>
    public class TextChannelServices : Services<TextChannelModel>
    {
        public TextChannelServices() : base("Main", "TextChannels") { }
        
    }
}

