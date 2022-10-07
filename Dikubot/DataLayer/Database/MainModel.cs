using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Dikubot.DataLayer.Database
{
    /// <summary>
    /// Class for elements in collection.
    /// </summary>
    public abstract class MainModel 
    {
        [BsonIgnoreIfDefault]
        [BsonId(IdGenerator = typeof(GuidGenerator))] [HiddenInput] [Display(Order = -1)]
        public Guid Id { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MainModel model && this.Id.Equals(model.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public virtual List<string> GetSearchContent()
        {
            return new List<string>();
        }

        public virtual HashSet<Guid> GetTags()
        {
            return new HashSet<Guid>();
        }
    }
}