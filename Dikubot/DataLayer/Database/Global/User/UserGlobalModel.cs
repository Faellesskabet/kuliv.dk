using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Calendar;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.User
{
    public class UserGlobalModel : MainModel
    {
        private string _email;

        [BsonElement("Email")]
        [BsonUnique]
        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }

        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("DiscordId")]
        [BsonUnique]
        public string DiscordId { get; set; }

        [BsonIgnore]
        public IUser DiscordUser
        {
            get => DiscordBot.ClientStatic.GetUserAsync(DiscordIdLong).Result;
            set => DiscordId = value.Id.ToString();
        }
        
        [BsonIgnore]
        public ulong DiscordIdLong
        {
            get => Convert.ToUInt64(DiscordId);
            set => DiscordId = value.ToString();
        }

        [BsonElement("Verified")] public bool Verified { get; set; } = false;
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

        [BsonElement("IsAdmin")]
        public bool IsAdmin
        {
            get;
            set;
        }

        [BsonElement("Calendar")]
        public HashSet<Guid> Calendar
        {
            get;
            set;
        }

        [BsonElement("IsBot")] public bool IsBot { get; set; }
        [BsonElement("Username")] public string Username { get; set; }
        [BsonElement("JoinedAt")] public DateTime JoinedAt { get; set; } = DateTime.Now;

        [BsonElement("Banned")] public bool IsBanned { get; set; } = false;
        [BsonElement("SelectedGuild")] public ulong SelectedGuild { get; set; }
        
        public SocketGuild GetSelectedGuild()
        {
            SocketGuild guild = DiscordBot.ClientStatic.GetGuild(SelectedGuild);
            if (guild == null)
            {
                throw new Exception($"SelectedGuild ({SelectedGuild}) can't be found");
            }
            return guild;
        }
        
        [BsonElement("DarkTheme")]
        public bool DarkTheme { get; set; }


        /// <summary>
        /// Get all guids the user
        /// </summary>
        [BsonIgnore]
        public HashSet<SocketGuild> Guilds
        {
            get => DiscordId.IsNullOrEmpty()
                ? new HashSet<SocketGuild>()
                : DiscordBot.ClientStatic.GetUser(this.DiscordIdLong).MutualGuilds.ToHashSet();
        }
        
        
        /// <summary>
        /// Get all guid for roles the user have
        /// </summary>
        public HashSet<Guid> GetRolesGuid()
        {
            HashSet<Guid> result = Guilds.SelectMany(model => GetRolesGuid(model.Id)).ToHashSet();
            return result;
        }
        public HashSet<Guid> GetRolesGuid(ulong guildId)
        {
            
            var guild = DiscordBot.ClientStatic.GetGuild((ulong) guildId);
            return GetRolesGuid(guild);
        }
        
        public HashSet<Guid> GetRolesGuid(SocketGuild guild)
        {
            UserGuildServices userGuildServices = new UserGuildServices(guild);
            return userGuildServices
                .Get(model => model.DiscordId.Equals(this.DiscordId))
                .Roles.Select(model => model.RoleId)
                .ToHashSet();
        }
        
        
        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllViewCalenders(CalendarModel.EnumCalendarType calendarType)
        {
            var roles = GetRolesGuid() ?? new HashSet<Guid>();

            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();
            
            foreach (var socketGuild in DiscordBot.ClientStatic.Guilds)
            {
                var service = new CalendarServices(socketGuild);
            
                List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => 
                    model.Visible == CalendarModel.EnumAvailable.Public 
                    || (Verified && model.Visible == CalendarModel.EnumAvailable.Externt)
                    || (Guilds.Contains(socketGuild) && model.Visible == CalendarModel.EnumAvailable.Internt)
                    || model.Permission.Overlaps(roles)
                    || model.View.Overlaps(roles)).ToList();
                result.Add((socketGuild,value));
            }
            return result;
        }
        
        
        
        async public Task<List<(SocketGuild, List<CalendarModel>)>> GetAllPermisionsCalendars(CalendarModel.EnumCalendarType calendarType)
        {
            
            if (Verified is false)
            {
                return new List<(SocketGuild, List<CalendarModel>)>();
            }
            
            var roles = GetRolesGuid();

            List<(SocketGuild,List<CalendarModel>)> result = new List<(SocketGuild, List<CalendarModel>)>();
            
            
            foreach (var socketGuild in Guilds)
            {
                var service = new CalendarServices(socketGuild.Id.ToString());
            
                
                List<CalendarModel> value = service.GetAll(model => model.CalendarType == calendarType).Where(model => model.Permission.Overlaps(roles)).ToList();
                result.Add((socketGuild,value));
                 
                
            }

            return result;

        }
        
    }
    
}
