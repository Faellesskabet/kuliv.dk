using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.Color
{
    public class ColorModel
    {
        public ColorModel(global::Discord.Color? color = null)
        {
            ToModel(color);
        }

        [BsonElement("Red")] public int Red { get; set; }
        [BsonElement("Green")] public int Green { get; set; }
        [BsonElement("Blue")] public int Blue { get; set; }

        public global::Discord.Color ToColor() =>
            new(Red, Green, Blue);

        public void ToModel(global::Discord.Color? color)
        {
            if (color == null)
                return;

            Red = Convert.ToInt32(color.Value.R);
            Green = Convert.ToInt32(color.Value.G);
            Blue = Convert.ToInt32(color.Value.B);
        }
        
        public override string ToString()
        {
            return ToString("A");
        }

        public string ToString(string fmt)
        {
            if (string.IsNullOrEmpty(fmt))
                fmt = "A";

            switch (fmt.ToUpperInvariant())
            {
                case "r":
                    return string.Format("{0}",Red);
                case "g":
                    return string.Format("{0}",Green);
                case "b":
                    return string.Format("{0}",Blue);
                case "A":
                    return string.Format("R:{0}, G:{1}, B:{2}",Red, Green, Blue);
                default:
                    string msg = string.Format("'{0}' is an invalid format string",
                        fmt);
                    throw new ArgumentException(msg);
            }
        }
        
    }
}