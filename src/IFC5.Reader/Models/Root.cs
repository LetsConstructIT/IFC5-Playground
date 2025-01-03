using IFC5.Reader.Models.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IFC5.Reader.Models;

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

        return null;
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
