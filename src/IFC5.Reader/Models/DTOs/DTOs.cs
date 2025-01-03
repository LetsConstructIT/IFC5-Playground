using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFC5.Reader.Models.DTOs;
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
}

public partial class ComponentJson
{
    public virtual bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = null;
        return false;
    }
}

public partial class Ifc5ClassComponent : ComponentJson
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("uri")]
    public string? Uri { get; set; }

    public override bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = new Dictionary<string, string>
        {
            [$"{nameof(Ifc5ClassComponent)}:Code"] = Code ?? string.Empty,
            [$"{nameof(Ifc5ClassComponent)}:uri"] = Uri ?? string.Empty
        };

        return true;
    }
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

    public override bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = new Dictionary<string, string>
        {
            [$"{nameof(UsdGeomMeshComponent)}:faceVertexIndices"] = $"[{string.Join(",", FaceVertexIndices!)}]"
        };

        if (FaceVertexCounts is not null)
            attributes[$"{nameof(UsdGeomMeshComponent)}:faceVertexCounts"] = $"[{string.Join(",", FaceVertexCounts!)}]";

        var sb = new StringBuilder();
        sb.Append('[');
        foreach (var point in Points!)
        {
            sb.Append($"[{string.Join(",", point)}],");
        }
        sb.Length--;
        sb.Append(']');

        attributes[$"{nameof(UsdGeomMeshComponent)}:points"] = sb.ToString();

        return true;
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

    public override bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = new Dictionary<string, string>
        {
            [$"{nameof(UsdGeomVisibilityApiVisibilityComponent)}:Visibility"] = Visibility ?? string.Empty
        };

        return true;
    }
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

    public override bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = new Dictionary<string, string>
        {
            [$"{nameof(Ifc5SpaceboundaryComponent)}:RelatedElement:ref"] = RelatedElement!.Ref!
        };

        return true;
    }
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

    public override bool TryConvertToAttributes(out Dictionary<string, string>? attributes)
    {
        attributes = new Dictionary<string, string>
        {
            [$"{nameof(CustomDataComponent)}:OriginalStepInstance"] = OriginalStepInstance ?? string.Empty
        };

        return true;
    }
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