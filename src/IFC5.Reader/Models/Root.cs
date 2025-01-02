using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IFC5.Reader.Models;

public partial class Root : List<PrimJson>
{
}

public partial class Disclaimer
{
    [JsonProperty("disclaimer")]
    public string? Note { get; set; }
}

public partial class PrimJson
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("inherits")]
    public string[]? Inherits { get; set; }

    public string? GetValidInherit()
    {
        if (Inherits is null || Inherits.Length != 1)
            return null;

        var inherit = Inherits[0];
        var offset = 2;
        return inherit.Substring(offset, inherit.Length - offset - 1);
    }
}

public interface IParent
{
    public DefJson[]? Children { get; set; }
}

public interface IComponentable
{
    public ComponentJson? Component { get; set; }
}

public partial class ComponentJson
{

}
public partial class NullComponent : ComponentJson
{
    private static readonly NullComponent _instance = new NullComponent();

    private NullComponent() { }

    public static NullComponent Instance => _instance;
}

public partial class Ifc5ClassComponent : ComponentJson
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uri")]
    public string? Uri { get; set; }
}

public partial class NlsfbClassComponent : ComponentJson
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uri")]
    public Uri? Uri { get; set; }
}

public partial class Ifc5PropertiesComponent : ComponentJson
{
    [JsonProperty("IsExternal")]
    public long IsExternal { get; set; }
}

public partial class UsdShadeMaterialBindingApiComponent : ComponentJson
{
    [JsonProperty("material:binding")]
    public OutputsSurfaceConnectComponent? MaterialBinding { get; set; }
}

public partial class OutputsSurfaceConnectComponent : ComponentJson
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }
}

public partial class UsdGeomMeshComponent : ComponentJson
{
    [JsonProperty("faceVertexIndices")]
    public int[]? FaceVertexIndices { get; set; }

    [JsonProperty("points")]
    public double[][]? Points { get; set; }

    [JsonProperty("faceVertexCounts")]
    public int[]? FaceVertexCounts { get; set; }

    public int GetFaceVertexCount()
    {
        if (FaceVertexCounts == null)
            return 3;

        return FaceVertexCounts[0];
    }
}

public partial class UsdGeomBasisCurvesComponent : ComponentJson
{
    [JsonProperty("points")]
    public long[][]? Points { get; set; }
}

public partial class XformOpComponent : ComponentJson
{
    [JsonProperty("transform")]
    public double[][]? Transform { get; set; }
}

public partial class UsdGeomVisibilityApiVisibilityComponent : ComponentJson
{
    [JsonProperty("visibility")]
    public string? Visibility { get; set; }
}

public partial class UsdShadeShaderComponent : ComponentJson
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

public partial class Ifc5SpaceboundaryComponent : ComponentJson
{
    [JsonProperty("relatedElement")]
    public OutputsSurfaceConnect? RelatedElement { get; set; }
}

public partial class OutputsSurfaceConnect
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }
}

public partial class CustomDataComponent : ComponentJson
{
    [JsonProperty("originalStepInstance")]
    public string? OriginalStepInstance { get; set; }
}

public partial class DefJson : PrimJson, IParent, IComponentable
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("children")]
    public DefJson[]? Children { get; set; }

    [JsonProperty("attributes")]
    public ComponentJson? Component { get; set; }
}

public partial class ClassJson : PrimJson, IParent
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("children")]
    public DefJson[]? Children { get; set; }
}

public partial class OverJson : PrimJson, IComponentable
{
    [JsonProperty("attributes")]
    public ComponentJson? Component { get; set; }
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

internal class PrimConverter : JsonConverter<PrimJson>
{
    public override PrimJson? ReadJson(JsonReader reader, Type objectType, PrimJson? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObj = JObject.Load(reader);

        return ReadPrim(jObj);
    }

    private PrimJson? ReadPrim(JObject jObj)
    {
        if (!jObj.ContainsKey("def"))
            return null;

        var readObject = jObj.ToObject(GetDefType(jObj.Value<string>("def")!));

        if (readObject is not PrimJson prim)
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

    private DefJson[]? ReadChildren(JObject jObj)
    {
        if (!jObj.ContainsKey("children"))
            return null;

        var jChildren = jObj["children"];
        if (jChildren is not JArray array)
            return null;

        var children = new List<DefJson>();
        foreach (var child in array)
        {
            if (child is not JObject jObject)
                continue;

            var prim = ReadPrim(jObject);
            if (prim is DefJson def)
                children.Add(def);
        }

        return children.ToArray();
    }

    private ComponentJson? ReadComponent(JObject jObj)
    {
        if (!jObj.ContainsKey("attributes"))
            return null;

        var jChildren = jObj["attributes"];
        if (jChildren != null && jChildren.Count() == 1 && jChildren.First() is JProperty property)
        {
            var componentType = GetComponentType(property.Name);
            if (componentType is null)
                return null;

            var readObject = property.Value.ToObject(componentType);
            if (readObject is ComponentJson component)
                return component;
        }
        else // strange UsdShade:Shader, not wrapped in single property
        {
            var readObject = jChildren!.ToObject(typeof(UsdShadeShaderComponent));
            if (readObject is ComponentJson component)
                return component;
        }
        return new ComponentJson();
    }

    private Type GetDefType(string typeName)
    {
        return typeName switch
        {
            "def" => typeof(DefJson),
            "class" => typeof(ClassJson),
            "over" => typeof(OverJson),
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

    public override void WriteJson(JsonWriter writer, PrimJson? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    private static readonly PrimConverter _singleton = new();
    public static PrimConverter Singleton => _singleton;
}
