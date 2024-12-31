namespace IFC5Tekla.Engine.Models;

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public partial class IfcX
{
    [JsonProperty("disclaimer", NullValueHandling = NullValueHandling.Ignore)]
    public string Disclaimer { get; set; }

    [JsonProperty("def", NullValueHandling = NullValueHandling.Ignore)]
    public PrimType? PrimType { get; set; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public WelcomeType? Type { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
    public Child[] Children { get; set; }

    [JsonProperty("inherits", NullValueHandling = NullValueHandling.Ignore)]
    public string[] Inherits { get; set; }

    [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
    public string Comment { get; set; }

    [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
    public Components Attributes { get; set; }
}

public partial class Components
{
    [JsonProperty("customdata", NullValueHandling = NullValueHandling.Ignore)]
    public Customdata Customdata { get; set; }

    [JsonProperty("ifc5:class", NullValueHandling = NullValueHandling.Ignore)]
    public NlsfbClass Ifc5Class { get; set; }

    [JsonProperty("UsdGeom:Mesh", NullValueHandling = NullValueHandling.Ignore)]
    public UsdGeomMesh UsdGeomMesh { get; set; }

    [JsonProperty("UsdGeom:VisibilityAPI:visibility", NullValueHandling = NullValueHandling.Ignore)]
    public UsdGeomVisibilityApiVisibility UsdGeomVisibilityApiVisibility { get; set; }

    [JsonProperty("UsdShade:MaterialBindingAPI", NullValueHandling = NullValueHandling.Ignore)]
    public UsdShadeMaterialBindingApi UsdShadeMaterialBindingApi { get; set; }

    [JsonProperty("xformOp", NullValueHandling = NullValueHandling.Ignore)]
    public XformOp XformOp { get; set; }

    [JsonProperty("ifc5:properties", NullValueHandling = NullValueHandling.Ignore)]
    public Ifc5Properties Ifc5Properties { get; set; }

    [JsonProperty("nlsfb:class", NullValueHandling = NullValueHandling.Ignore)]
    public NlsfbClass NlsfbClass { get; set; }

    [JsonProperty("UsdGeom:BasisCurves", NullValueHandling = NullValueHandling.Ignore)]
    public UsdGeomBasisCurves UsdGeomBasisCurves { get; set; }

    [JsonProperty("ifc5:spaceboundary", NullValueHandling = NullValueHandling.Ignore)]
    public Ifc5Spaceboundary Ifc5Spaceboundary { get; set; }

    [JsonProperty("UsdShade:Material", NullValueHandling = NullValueHandling.Ignore)]
    public UsdShadeMaterial UsdShadeMaterial { get; set; }
}

public partial class Customdata
{
    [JsonProperty("originalStepInstance")]
    public string OriginalStepInstance { get; set; }
}

public partial class NlsfbClass
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("uri")]
    public Uri Uri { get; set; }
}

public partial class Ifc5Properties
{
    [JsonProperty("IsExternal")]
    public long IsExternal { get; set; }
}

public partial class Ifc5Spaceboundary
{
    [JsonProperty("relatedElement")]
    public OutputsSurfaceConnect RelatedElement { get; set; }
}

public partial class OutputsSurfaceConnect
{
    [JsonProperty("ref")]
    public string Ref { get; set; }
}

public partial class UsdGeomBasisCurves
{
    [JsonProperty("points")]
    public long[][] Points { get; set; }
}

public partial class UsdGeomMesh
{
    [JsonProperty("faceVertexIndices")]
    public long[] FaceVertexIndices { get; set; }

    [JsonProperty("points")]
    public double[][] Points { get; set; }

    [JsonProperty("faceVertexCounts", NullValueHandling = NullValueHandling.Ignore)]
    public long[] FaceVertexCounts { get; set; }
}

public partial class UsdGeomVisibilityApiVisibility
{
    [JsonProperty("visibility")]
    public string Visibility { get; set; }
}

public partial class UsdShadeMaterial
{
    [JsonProperty("outputs:surface.connect")]
    public OutputsSurfaceConnect OutputsSurfaceConnect { get; set; }
}

public partial class UsdShadeMaterialBindingApi
{
    [JsonProperty("material:binding")]
    public OutputsSurfaceConnect MaterialBinding { get; set; }
}

public partial class XformOp
{
    [JsonProperty("transform")]
    public double[][] Transform { get; set; }
}

public partial class Child
{
    [JsonProperty("def")]
    public PrimType Def { get; set; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public ChildType? Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("inherits", NullValueHandling = NullValueHandling.Ignore)]
    public string[] Inherits { get; set; }

    [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
    public ChildAttributes Attributes { get; set; }
}

public partial class ChildAttributes
{
    [JsonProperty("info:id")]
    public string InfoId { get; set; }

    [JsonProperty("inputs:diffuseColor")]
    public double[] InputsDiffuseColor { get; set; }

    [JsonProperty("inputs:opacity")]
    public double InputsOpacity { get; set; }

    [JsonProperty("outputs:surface")]
    public object OutputsSurface { get; set; }
}

public enum PrimType { Class, Def, Over };

public enum ChildType { UsdGeomBasisCurves, UsdGeomMesh, UsdShadeShader };

public enum WelcomeType { UsdGeomBasisCurves, UsdGeomMesh, UsdGeomXform, UsdShadeMaterial };

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            DefConverter.Singleton,
            ChildTypeConverter.Singleton,
            WelcomeTypeConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

internal class DefConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(PrimType) || t == typeof(PrimType?);

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "class":
                return PrimType.Class;
            case "def":
                return PrimType.Def;
            case "over":
                return PrimType.Over;
        }
        throw new Exception("Cannot unmarshal type Def");
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (PrimType)untypedValue;
        switch (value)
        {
            case PrimType.Class:
                serializer.Serialize(writer, "class");
                return;
            case PrimType.Def:
                serializer.Serialize(writer, "def");
                return;
            case PrimType.Over:
                serializer.Serialize(writer, "over");
                return;
        }
        throw new Exception("Cannot marshal type Def");
    }

    public static readonly DefConverter Singleton = new DefConverter();
}

internal class ChildTypeConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(ChildType) || t == typeof(ChildType?);

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "UsdGeom:BasisCurves":
                return ChildType.UsdGeomBasisCurves;
            case "UsdGeom:Mesh":
                return ChildType.UsdGeomMesh;
            case "UsdShade:Shader":
                return ChildType.UsdShadeShader;
        }
        throw new Exception("Cannot unmarshal type ChildType");
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (ChildType)untypedValue;
        switch (value)
        {
            case ChildType.UsdGeomBasisCurves:
                serializer.Serialize(writer, "UsdGeom:BasisCurves");
                return;
            case ChildType.UsdGeomMesh:
                serializer.Serialize(writer, "UsdGeom:Mesh");
                return;
            case ChildType.UsdShadeShader:
                serializer.Serialize(writer, "UsdShade:Shader");
                return;
        }
        throw new Exception("Cannot marshal type ChildType");
    }

    public static readonly ChildTypeConverter Singleton = new ChildTypeConverter();
}

internal class WelcomeTypeConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(WelcomeType) || t == typeof(WelcomeType?);

    public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "UsdGeom:BasisCurves":
                return WelcomeType.UsdGeomBasisCurves;
            case "UsdGeom:Mesh":
                return WelcomeType.UsdGeomMesh;
            case "UsdGeom:Xform":
                return WelcomeType.UsdGeomXform;
            case "UsdShade:Material":
                return WelcomeType.UsdShadeMaterial;
        }
        throw new Exception("Cannot unmarshal type WelcomeType");
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (WelcomeType)untypedValue;
        switch (value)
        {
            case WelcomeType.UsdGeomBasisCurves:
                serializer.Serialize(writer, "UsdGeom:BasisCurves");
                return;
            case WelcomeType.UsdGeomMesh:
                serializer.Serialize(writer, "UsdGeom:Mesh");
                return;
            case WelcomeType.UsdGeomXform:
                serializer.Serialize(writer, "UsdGeom:Xform");
                return;
            case WelcomeType.UsdShadeMaterial:
                serializer.Serialize(writer, "UsdShade:Material");
                return;
        }
        throw new Exception("Cannot marshal type WelcomeType");
    }

    public static readonly WelcomeTypeConverter Singleton = new WelcomeTypeConverter();
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.