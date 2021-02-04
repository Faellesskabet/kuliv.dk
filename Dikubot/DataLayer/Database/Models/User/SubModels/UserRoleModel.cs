using System;
using System.Transactions;
using Dikubot.Database.Models.Interfaces;
using Dikubot.Database.Models.Role;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.SubModels
{
    public class UserRoleModel : IActiveTimeFrame
    {
        private Guid _roleId;
        private static readonly RoleServices _roleServices = new RoleServices();

        public UserRoleModel(RoleModel roleModel) : this(roleModel.Id)
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

        [BsonIgnore]
        public RoleModel RoleModel
        {
            get => _roleServices.Get(_roleId);
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

        [BsonElement("StartDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime StartDate { get; set; }
        
        [BsonElement("EndDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime EndDate { get; set; }
    }
}