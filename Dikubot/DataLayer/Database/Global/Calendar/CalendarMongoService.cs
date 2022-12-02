using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Database.Guild.Models.User.SubModels;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.Calendar
{
    public class CalendarMongoService : GlobalMongoService<CalendarModel>
    {

        private readonly DiscordSocketClient _discordBotClient;
        private readonly IGuildMongoFactory _guildMongoFactory;
        public CalendarMongoService(Database database, DiscordSocketClient discordBotClient, IGuildMongoFactory guildMongoFactory) : base(database)
        {
            _discordBotClient = discordBotClient;
            _guildMongoFactory = guildMongoFactory;

        }
        
        public List<CalendarModel> Get(CalendarModel.EnumCalendarType calendarType, UserGlobalModel user)
        {

            SocketUser socketUser = _discordBotClient.GetUser(user.DiscordIdLong);
            IEnumerable<Guid> userRoleGuids = new List<Guid>();
            if (socketUser != null)
            {
                userRoleGuids = socketUser.MutualGuilds.SelectMany(guild =>
                    _guildMongoFactory.Get<UserGuildMongoService>(guild).Get(socketUser).Roles).Select(model => model.RoleId);
            }
            
            return GetAll(model => model.CalendarType == calendarType)
                .FindAll(model =>
                {
                    var roleGuids = userRoleGuids.ToList();
                    return model.Visible != CalendarModel.EnumAvailable.Privat
                           || model.Permission.Overlaps(roleGuids)
                           || model.View.Overlaps(roleGuids);
                });
        }

        public override string GetCollectionName()
        {
            return "Calendar";
        }
    }

    

}