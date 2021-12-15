using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Dikubot.DataLayer.Database.Models
{
    /// <summary>
    /// Class for elements in collection.
    /// </summary>
    public abstract class Model 
    {

        [BsonIgnoreIfDefault]
        [BsonId(IdGenerator = typeof(GuidGenerator))] [HiddenInput]
        public Guid Id { get; set; }
        
        public bool Equals(Model model)
        {
            return this.Equals(model) || this.Id.Equals(model?.Id);
        }
        
        public object GetDisplayValue(ModelMetadata metadata)
        {
            
            if (!string.IsNullOrWhiteSpace(metadata.DisplayFormatString))
            {
                return string.Format(metadata.DisplayFormatString,
                    this.GetType()?.GetProperty(metadata.Name)?.GetValue(this, null));
            }
            return this.GetType()?.GetProperty(metadata.Name)?.GetValue(this, null) ?? "";
        }

        /*
        public object GetMetedata()
        {
              Metadata  MetadataProvider.GetMetadataForProperties(item.GetType());
        }
        */

    }
}