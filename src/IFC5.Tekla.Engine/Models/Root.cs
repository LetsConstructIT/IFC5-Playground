using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace IFC5Tekla.Engine.ModelsNew;

public partial class Root : List<Prim>
{
}

public partial class Component
{

}

public partial class Disclaimer : Component
{
    [JsonProperty("disclaimer")]
    public string? Note { get; set; }
}

public partial class Prim : Component
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("inherits")]
    public string[]? Inherits { get; set; }

    [JsonProperty("children")]
    public Prim[]? Children { get; set; }
}

public partial class Def : Prim
{
    [JsonProperty("type")]
    public string? Type { get; set; }
}

public partial class Class : Prim
{
    [JsonProperty("type")]
    public string? Type { get; set; }
}

public partial class Over : Prim
{

}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            PrimConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

internal class PrimConverter : JsonConverter<Prim>
{
    public override Prim? ReadJson(JsonReader reader, Type objectType, Prim? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObj = JObject.Load(reader);

        return ReadPrim(jObj);
    }

    private Prim? ReadPrim(JObject jObj)
    {
        if (!jObj.ContainsKey("def"))
            return null;

        var readObject = jObj.ToObject(GetDefType(jObj.Value<string>("def")!));

        if (readObject is not Prim prim)
            return null;

        var children = ReadChildren(jObj);
        prim.Children = children;
        return prim;
    }

    private Prim[]? ReadChildren(JObject jObj)
    {
        if (!jObj.ContainsKey("children"))
            return null;

        var jChildren = jObj["children"];
        if (jChildren is not JArray array)
            return null;

        var children = new List<Prim>();
        foreach (var child in array)
        {
            if (child is not JObject jObject)
                continue;

            var prim = ReadPrim(jObject);
            if (prim is not null)
                children.Add(prim);
        }

        return children.ToArray();
    }

    private Type GetDefType(string typeName)
    {
        return typeName switch
        {
            "def" => typeof(Def),
            "class" => typeof(Class),
            "over" => typeof(Over),
            _ => throw new InvalidCastException()
        };
    }

    public override void WriteJson(JsonWriter writer, Prim? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    private static readonly PrimConverter _singleton = new();
    public static PrimConverter Singleton => _singleton;
}