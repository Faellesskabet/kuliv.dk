using System;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Interfaces;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.User.SubModels
{
    public class UserRoleModel : IActiveTimeFrame
    {
        //Todo: Se om dette fungere :/
        
        private Guid _roleId;
        
        public UserRoleModel(RoleMainModel roleMainModel) : this(roleMainModel.Id)
        {
        }

        public UserRoleModel(Guid roleId)
        {
            _roleId = roleId;
        }

        [BsonElement("RoleId")]
        [BsonRequired]
        public Guid RoleId
        {
            get => _roleId;
            set => _roleId = value;
        }
        
        
        public RoleMainModel GetRoleModel(SocketGuild guild)
        {
            return new RoleMongoService(guild).Get(_roleId);
        }


        /// <summary>
        /// We override the Equals function to determine two UserRoleModels are only equal if their role Id is the same
        /// </summary>
        /// <param name="obj">Object of any kind</param>
        /// <returns>Returns true if the object is an instance of UserRoleModel and if the RoleId is the same for both objects, otherwise it will return false</returns>
        public override bool Equals(object obj)
        {
            return (obj is UserRoleModel model) && (model.RoleId == this.RoleId);
        }

        /// <summary>
        /// The hashcode of our object is simply the roleId's hashcode. It is important for stuff such as Dicts and HashSets
        /// </summary>
        /// <returns>Hashcode of roleId</returns>
        public override int GetHashCode()
        {
            return _roleId.GetHashCode();
        }

        
        /// <summary>
        /// Start and End Date, if not set, they are set to min and max
        /// </summary>
        [BsonElement("StartDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        
        [BsonElement("EndDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
    }
}