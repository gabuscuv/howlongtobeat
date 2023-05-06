using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HowLongToBeat
{

    public class Game
    {
        [JsonProperty("game_name")]
        public string Title { get; }
        [JsonProperty("game_image"), JsonConverter(typeof(ImageURLConverter))]
        public string ImgURL { get; }
        [JsonProperty("comp_main"), JsonConverter(typeof(TimeConverter))]
        public string Main { get; }
        [JsonProperty("comp_plus"), JsonConverter(typeof(TimeConverter))]
        public string MainAndExtras { get; }
        [JsonProperty("comp_100"), JsonConverter(typeof(TimeConverter))]
        public string Completionist { get; }

        public Game(string title, string main, string mainandextras, string completionist, string imgURL)
        {
            Title = title;
            ImgURL = imgURL;
            Main = main;
            MainAndExtras = mainandextras;
            Completionist = completionist;
        }
    }

    internal class ImageURLConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) { return; }
            string[] _value = ((String)value).Split("/");

            writer.WriteValue(_value[_value.Length]);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return "https://howlongtobeat.com/games/" + (String)reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }

    internal class TimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) { return; }
            writer.WriteValue(Math.Round((decimal)value * 3600, 2).ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return Math.Round((Decimal)((Int64)reader.Value) / 3600, 2).ToString();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Decimal) || objectType == typeof(System.Int64);
        }

    }
}
