using IFC5Tekla.Engine.Domain;
using IFC5Tekla.Engine.Exceptions;

namespace IFC5Tekla.Engine.Models;
public static class Mappers
{
    public static Def ToDomain(this DefJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        if (json.Type is null || string.IsNullOrEmpty(json.Type))
            throw new MappingException(nameof(json.Type));

        var component = json.Component is null ? new NullComponent() : json.Component;
        return new Def(json.Name, json.Type, component);
    }

    public static Class ToDomain(this ClassJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        if (json.Type is null || string.IsNullOrEmpty(json.Type))
            throw new MappingException(nameof(json.Type));

        return new Class(json.Name, json.Type);
    }

    public static Over ToDomain(this OverJson json)
    {
        if (json.Name is null || string.IsNullOrEmpty(json.Name))
            throw new MappingException(nameof(json.Name));

        var component = json.Component is null ? new NullComponent() : json.Component;
        return new Over(json.Name, component);
    }
}
