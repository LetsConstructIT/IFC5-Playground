using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IFC5Tekla.Engine.Models;

public partial class Root : List<Prim>
{
}

public partial class Disclaimer
{
    [JsonProperty("disclaimer")]
    public string? Note { get; set; }
}

public partial class Prim
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("inherits")]
    public string[]? Inherits { get; set; }
}

public interface IParent
{
    public Def[]? Children { get; set; }
}

public interface IComponentable
{
    public Component? Component { get; set; }
}

public partial class Component
{

}

public partial class Ifc5ClassComponent : Component
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uri")]
    public string? Uri { get; set; }
}

public partial class NlsfbClassComponent : Component
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uri")]
    public Uri? Uri { get; set; }
}

public partial class Ifc5PropertiesComponent : Component
{
    [JsonProperty("IsExternal")]
    public long IsExternal { get; set; }
}

public partial class UsdShadeMaterialBindingApiComponent : Component
{
    [JsonProperty("material:binding")]
    public OutputsSurfaceConnectComponent? MaterialBinding { get; set; }
}

public partial class OutputsSurfaceConnectComponent : Component
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }
}

public partial class UsdGeomMeshComponent : Component
{
    [JsonProperty("faceVertexIndices")]
    public long[]? FaceVertexIndices { get; set; }

    [JsonProperty("points")]
    public double[][]? Points { get; set; }
}

public partial class UsdGeomBasisCurvesComponent : Component
{
    [JsonProperty("points")]
    public long[][]? Points { get; set; }
}

public partial class XformOpComponent : Component
{
    [JsonProperty("transform")]
    public double[][]? Transform { get; set; }
}

public partial class UsdGeomVisibilityApiVisibilityComponent : Component
{
    [JsonProperty("visibility")]
    public string? Visibility { get; set; }
}

public partial class UsdShadeShaderComponent : Component
{
    [JsonProperty("info:id")]
    public string? Id { get; set; }
    [JsonProperty("inputs:diffuseColor")]
    public double[]? DiffuseColor { get; set; }
    [JsonProperty("inputs:opacity")]
    public double? Opacity { get; set; }
    [JsonProperty("outputs:surface")]
    public string? Surface { get; set; }
}

public partial class Ifc5SpaceboundaryComponent : Component
{
    [JsonProperty("relatedElement")]
    public OutputsSurfaceConnect? RelatedElement { get; set; }
}

public partial class OutputsSurfaceConnect
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }
}

public partial class CustomDataComponent : Component
{
    [JsonProperty("originalStepInstance")]
    public string? OriginalStepInstance { get; set; }
}

public partial class Def : Prim, IParent, IComponentable
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("children")]
    public Def[]? Children { get; set; }

    [JsonProperty("attributes")]
    public Component? Component { get; set; }
}

public partial class Class : Prim, IParent
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("children")]
    public Def[]? Children { get; set; }
}

public partial class Over : Prim, IComponentable
{
    [JsonProperty("attributes")]
    public Component? Component { get; set; }
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

        if (readObject is IParent parent)
        {
            parent.Children = ReadChildren(jObj);
        }

        if (readObject is IComponentable componentable)
        {
            componentable.Component = ReadComponent(jObj);
        }

        return prim;
    }

    private Def[]? ReadChildren(JObject jObj)
    {
        if (!jObj.ContainsKey("children"))
            return null;

        var jChildren = jObj["children"];
        if (jChildren is not JArray array)
            return null;

        var children = new List<Def>();
        foreach (var child in array)
        {
            if (child is not JObject jObject)
                continue;

            var prim = ReadPrim(jObject);
            if (prim is Def def)
                children.Add(def);
        }

        return children.ToArray();
    }

    private Component? ReadComponent(JObject jObj)
    {
        if (!jObj.ContainsKey("attributes"))
            return null;

        var jChildren = jObj["attributes"];
        if (jChildren.Count() == 1 && jChildren.First() is JProperty property)
        {
            var componentType = GetComponentType(property.Name);
            if (componentType is null)
                return null;

            var readObject = property.Value.ToObject(componentType);
            if (readObject is Component component)
                return component;
        }
        else // strange UsdShade:Shader, not wrapped in single property
        {
            var readObject = jChildren!.ToObject(typeof(UsdShadeShaderComponent));
            if (readObject is Component component)
                return component;
        }
        return new Component();
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

    private Type? GetComponentType(string componentName)
    {
        return componentName switch
        {
            "ifc5:class" => typeof(Ifc5ClassComponent),
            "ifc5:properties" => typeof(Ifc5PropertiesComponent),
            "ifc5:spaceboundary" => typeof(Ifc5SpaceboundaryComponent),
            "UsdShade:MaterialBindingAPI" => typeof(UsdShadeMaterialBindingApiComponent),
            "UsdGeom:Mesh" => typeof(UsdGeomMeshComponent),
            "UsdGeom:BasisCurves" => typeof(UsdGeomBasisCurvesComponent),
            "UsdGeom:VisibilityAPI:visibility" => typeof(UsdGeomVisibilityApiVisibilityComponent),
            "xformOp" => typeof(XformOpComponent),
            "nlsfb:class" => typeof(NlsfbClassComponent),
            "customdata" => typeof(CustomDataComponent),
            _ => null
        };
    }

    public override void WriteJson(JsonWriter writer, Prim? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    private static readonly PrimConverter _singleton = new();
    public static PrimConverter Singleton => _singleton;
}
