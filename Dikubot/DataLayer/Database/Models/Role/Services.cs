using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using Dikubot.Discord;
using Discord;
using MongoDB.Driver;

namespace Dikubot.Database.Models.Role
{
    /// <summary>
    /// Class for for retrieving information from the User collection.
    /// </summary>
    public class RoleServices : Services<RoleModel>
    {
        public RoleServices() : base("Main", "Roles") { }

        /// <Summary>Inserts a Model in the collection. If a RoleModel with the same ID, Name or discordID already
        /// exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new RoleModel Upsert(RoleModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool nameCollision = Exists(model => model.Name == modelIn.Name);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);

            if (idCollision)
            {
                Update(modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }
            if (nameCollision)
            {
                Update(m => m.Name == modelIn.Name, modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }
            if (discordIdCollision)
            {
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn, new ReplaceOptions() 
                    {IsUpsert = true});
                return modelIn;
            }
            Insert(modelIn);
            return modelIn;
        }
        
        /// <Summary>Checks if a RoleModel is already in the database.</Summary>
        /// <param name="modelIn">A boolean which tells if the models is in the database.</param>
        /// <return>A bool, true if the value already exist false if not.</return>
        public new bool Exists(RoleModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool nameCollision = Exists(model => model.Name == modelIn.Name);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);
            return idCollision || nameCollision || discordIdCollision;
        }
        
        /// <Summary>Removes a element from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(RoleModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
            Remove(model => model.Name == modelIn.Name);
        }

        /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
        /// <param name="role">The SocketRole model one wishes to be converted.</param>
        /// <return>A RoleModel.</return>
        public RoleModel SocketToModel(SocketRole role)
        {
            RoleModel _role = new RoleModel();
            Dictionary<string, bool> Permissions = new Dictionary<string, bool>();
            Permissions["Administrator"] = role.Permissions.Administrator;
            Permissions["Connect"] = role.Permissions.Connect;
            Permissions["Speak"] = role.Permissions.Speak;
            Permissions["Stream"] = role.Permissions.Stream;
            Permissions["AddReactions"] = role.Permissions.AddReactions;
            Permissions["AttachFiles"] = role.Permissions.AttachFiles;
            Permissions["BanMembers"] = role.Permissions.BanMembers;
            Permissions["ChangeNickname"] = role.Permissions.ChangeNickname;
            Permissions["DeafenMembers"] = role.Permissions.DeafenMembers;
            Permissions["EmbedLinks"] = role.Permissions.EmbedLinks;
            Permissions["KickMembers"] = role.Permissions.KickMembers;
            Permissions["ManageChannels"] = role.Permissions.ManageChannels;
            Permissions["ManageEmojis"] = role.Permissions.ManageEmojis;
            Permissions["ManageGuild"] = role.Permissions.ManageGuild;
            Permissions["ManageMessages"] = role.Permissions.ManageMessages;
            Permissions["ManageNicknames"] = role.Permissions.ManageNicknames;
            Permissions["ManageRoles"] = role.Permissions.ManageRoles;
            Permissions["ManageWebhooks"] = role.Permissions.ManageWebhooks;
            Permissions["MentionEveryone"] = role.Permissions.MentionEveryone;
            Permissions["MoveMembers"] = role.Permissions.MoveMembers;
            Permissions["MuteMembers"] = role.Permissions.MuteMembers;
            Permissions["PrioritySpeaker"] = role.Permissions.PrioritySpeaker;
            Permissions["SendMessages"] = role.Permissions.SendMessages;
            Permissions["ViewChannel"] = role.Permissions.ViewChannel;
            Permissions["CreateInstantInvite"] = role.Permissions.CreateInstantInvite;
            Permissions["ReadMessageHistory"] = role.Permissions.ReadMessageHistory;
            Permissions["UseExternalEmojis"] = role.Permissions.UseExternalEmojis;
            Permissions["ViewAuditLog"] = role.Permissions.ViewAuditLog;
            Permissions["UseVAD"] = role.Permissions.UseVAD;
            Permissions["SendTTSMessages"] = role.Permissions.SendTTSMessages;
            Dictionary<string, int> color = new Dictionary<string, int>();
            color["Red"] = Convert.ToInt32(role.Color.R);
            color["Green"] = Convert.ToInt32(role.Color.G);
            color["Blue"] = Convert.ToInt32(role.Color.B);
            _role.Color = color;
            _role.Name = role.Name;
            _role.Permissions = Permissions;
            _role.Position = role.Position;
            _role.CreatedAt = role.CreatedAt.DateTime;
            _role.IsEveryone = role.IsEveryone;
            _role.IsHoisted = role.IsHoisted;
            _role.IsManaged = role.IsManaged;
            _role.IsMentionable = role.IsMentionable;
            _role.DiscordId = role.Id.ToString();
            return _role;
        }

        /// <Summary>Converts a RoleModel to a RoleProperties class which can be used to modify a role or create a role.
        /// </Summary>
        /// <param name="roleModel">The RoleModel which will be used to create a RoleProperties.</param>
        /// <return>A RoleProperties.</return>
        public RoleProperties ModelToRoleProperties(RoleModel roleModel)
        {
            var properties = new RoleProperties();
            properties.Name = roleModel.Name;
            properties.Hoist = roleModel.IsHoisted;
            properties.Mentionable = roleModel.IsMentionable;
            properties.Position = roleModel.Position;
            var permissions = new GuildPermissions(
                createInstantInvite: roleModel.Permissions["CreateInstantInvite"],
                kickMembers: roleModel.Permissions["KickMembers"],
                banMembers: roleModel.Permissions["BanMembers"],
                administrator: roleModel.Permissions["Administrator"],
                manageChannels: roleModel.Permissions["ManageChannels"],
                manageGuild: roleModel.Permissions["ManageGuild"],
                addReactions: roleModel.Permissions["AddReactions"],
                viewAuditLog: roleModel.Permissions["ViewAuditLog"],
                viewChannel: roleModel.Permissions["ViewChannel"],
                sendMessages: roleModel.Permissions["SendMessages"],
                sendTTSMessages: roleModel.Permissions["SendTTSMessages"],
                manageMessages: roleModel.Permissions["ManageMessages"],
                embedLinks: roleModel.Permissions["EmbedLinks"],
                attachFiles: roleModel.Permissions["AttachFiles"],
                readMessageHistory: roleModel.Permissions["ReadMessageHistory"],
                mentionEveryone: roleModel.Permissions["MentionEveryone"],
                useExternalEmojis: roleModel.Permissions["UseExternalEmojis"],
                connect: roleModel.Permissions["Connect"],
                speak: roleModel.Permissions["Speak"],
                muteMembers: roleModel.Permissions["MuteMembers"],
                deafenMembers: roleModel.Permissions["DeafenMembers"],
                moveMembers: roleModel.Permissions["MoveMembers"],
                useVoiceActivation: roleModel.Permissions["UseVAD"],
                prioritySpeaker: roleModel.Permissions["PrioritySpeaker"],
                stream: roleModel.Permissions["Stream"],
                changeNickname: roleModel.Permissions["ChangeNickname"],
                manageNicknames: roleModel.Permissions["ManageNicknames"],
                manageRoles: roleModel.Permissions["ManageRoles"],
                manageWebhooks: roleModel.Permissions["ManageWebhooks"],
                manageEmojis: roleModel.Permissions["ManageEmojis"]);
            properties.Permissions = permissions;
            properties.Color = new Color(roleModel.Color["Red"], 
                    roleModel.Color["Green"], 
                    roleModel.Color["Blue"]);
            return properties;
        }
    }
}

