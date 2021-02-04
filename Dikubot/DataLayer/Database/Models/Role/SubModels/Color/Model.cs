using System;
using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Role.SubModels
{
    public class ColorModel
    {
        public ColorModel(Color? color = null)
        {
            ToModel(color);
        }

        [BsonElement("Red")] public int Red { get; set; }
        [BsonElement("Green")] public int Green { get; set; }
        [BsonElement("Blue")] public int Blue { get; set; }

        public Color ToColor() =>
            new(Red, Green, Blue);

        public void ToModel(Color? color)
        {
            if (color == null)
                return;

            Red = Convert.ToInt32(color.Value.R);
            Green = Convert.ToInt32(color.Value.G);
            Blue = Convert.ToInt32(color.Value.B);
        }
    }
}