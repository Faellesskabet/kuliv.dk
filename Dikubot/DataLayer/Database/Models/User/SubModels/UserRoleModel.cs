using System;
using Dikubot.Database.Models.Role;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.SubModels
{
    public class UserRoleModel
    {
        private Guid _roleId;
        private static readonly RoleServices _roleServices = new RoleServices();
        public UserRoleModel(RoleModel roleModel) : this(roleModel.Id) { }

        public UserRoleModel(Guid roleId)
        {
            _roleId = roleId;
        }

        [BsonElement("RoleId")] [BsonRequired]
        public Guid RoleId { get => _roleId;
            set => _roleId = value;
        }

        [BsonIgnore]
        public RoleModel RoleModel { get => _roleServices.Get(_roleId); }

        /*
        * NOTE: DateTime.MinValue is the default value for a DateTime
        */
        [BsonElement("StartDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime StartDate { get; set; }
        
        [BsonElement("EndDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Check if the role has started (or should've started)
        /// </summary>
        /// <returns>Returns whether the current time is past the role's startdate</returns>
        public bool HasStarted()
        {
            return DateTime.Now > StartDate;
        }

        /// <summary>
        /// Check if the role has ended (or should've ended)
        /// </summary>
        /// <returns>Returns whether the current time is past the role's enddate</returns>
        public bool HasEnded()
        {
            /*
             * NOTE: DateTime.MinValue is the default value for a DateTime
             */
            return DateTime.Now > EndDate && EndDate != DateTime.MinValue;
        }
        /// <summary>
        /// Tells us whether the current time is in between the startdate and enddate
        /// </summary>
        /// <returns>Returns true if HasStarted() is true and HasEnded() is false</returns>
        public bool IsActive()
        {
            return HasStarted() && !HasEnded();
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
    }
}