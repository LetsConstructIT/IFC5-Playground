using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace IFC5.Reader.Models;

internal static class Ifc5JsonConverter
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
