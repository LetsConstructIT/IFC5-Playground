using IFC5.Reader.Models.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace IFC5.Reader.Models;

internal class Ifc5JsonConverter
{
    public Root Deserialize(string rawJson)
    {
        var ifcContent = JsonConvert.DeserializeObject<Root>(rawJson, Settings)!;

        return ifcContent;
    }

    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
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
