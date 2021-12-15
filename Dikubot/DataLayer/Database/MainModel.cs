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
        

        public bool Equals(MainModel mainModel)
        {
            return this.Equals(mainModel) || this.Id.Equals(mainModel?.Id);
        }
        
        public string GetDisplayValue(ModelMetadata metadata)
        {
            
            if (!string.IsNullOrWhiteSpace(metadata?.DisplayFormatString))
            {
                return string.Format(metadata.DisplayFormatString,
                    this.GetType()?.GetProperty(metadata.Name)?.GetValue(this, null));
            }
            return this.GetType()?.GetProperty(metadata.Name)?.GetValue(this, null)?.ToString() ?? "";
        }
        

        public static IEnumerable<ModelMetadata> Metadatas(IModelMetadataProvider metadataProvider,Type modelType)
        {
            return  metadataProvider.GetMetadataForProperties(modelType).Where(m => m.Order > 0).ToList();
            //var metadatas = Html.MetadataProvider.GetMetadataForProperties(Model.ModelType);
        }
        
        public static Dictionary<string,string> Display(IModelMetadataProvider metadataProvider, MainModel model)
        {
            var test =Metadatas(metadataProvider, model.GetType());
            var test2 = new Dictionary<string,string>();
            foreach (var VARIABLE in test)
            {
                test2.Add(VARIABLE.Name ?? VARIABLE.DisplayName, model.GetDisplayValue(VARIABLE));
            }
            
            return test2;
            //var metadatas = Html.MetadataProvider.GetMetadataForProperties(Model.ModelType);
        }
        


    }
}