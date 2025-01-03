using IFC5.Reader.Domain;
using IFC5.Reader.Exceptions;
using System;

namespace IFC5.Reader.Models;
public static class Mappers
{
    public static Def ToDomain(this DefJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        if (json.Type is null || string.IsNullOrEmpty(json.Type))
            throw new MappingException(nameof(json.Type));

        var inherits = json.Inherits is null ? Array.Empty<string>() : json.Inherits;
        return new Def(json.Name, inherits, json.Type, json.Component);
    }

    public static Def ToDomain(this DefJson json, string overwrittenName)
    {
        var type = json.Type is null || string.IsNullOrEmpty(json.Type) ?
            string.Empty : json.Type;

        var inherits = json.Inherits is null ? Array.Empty<string>() : json.Inherits;
        return new Def(overwrittenName, inherits, type, json.Component);
    }

    public static Class ToDomain(this ClassJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        if (json.Type is null || string.IsNullOrEmpty(json.Type))
            throw new MappingException(nameof(json.Type));

        var inherits = json.Inherits is null ? Array.Empty<string>() : json.Inherits;
        return new Class(json.Name, inherits, json.Type);
    }

    public static Over ToDomain(this OverJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        return new Over(json.Name, json.Component);
    }

    public static Prim ToDomain(this PrimJson primJson)
    {
        return primJson switch
        {
            ClassJson json => json.ToDomain(),
            DefJson json => json.ToDomain(),
            OverJson json => json.ToDomain(),
            _ => throw new MappingException(nameof(primJson)),
        };
    }
}
