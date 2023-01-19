using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Dikubot.DataLayer.Database.Models;

/// <summary>
///     Class for elements in collection.
/// </summary>
public abstract class Model
{
    [BsonIgnoreIfDefault]
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    [HiddenInput]
    private Guid Id { get; set; }
    
    /// <summary>
    /// When deserializing a BsonDocument from the database, it might not have a matching field in our class.
    /// To prevent this from throwing an exception and thereby not deserializing properly, we catch all extra elements.
    /// This means CatchAll will contain a lot of legacy data. We catch it instead of throwing it away because in some
    /// edge cases we might still want to use the old data. However, we should focus on migrating data in CatchAll
    /// to a proper field, rather than using this as a "feature".
    /// </summary>
    [BsonExtraElements]
    public BsonDocument CatchAll { get; set; }

    public bool Equals(Model model)
    {
        return Equals(model) || Id.Equals(model?.Id);
    }

    public object GetDisplayValue(ModelMetadata metadata)
    {
        if (!string.IsNullOrWhiteSpace(metadata.DisplayFormatString))
            return string.Format(metadata.DisplayFormatString,
                GetType()?.GetProperty(metadata.Name)?.GetValue(this, null));
        return GetType()?.GetProperty(metadata.Name)?.GetValue(this, null) ?? "";
    }
}