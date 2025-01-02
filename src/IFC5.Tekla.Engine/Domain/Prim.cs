using IFC5Tekla.Engine.Models;
using System;

namespace IFC5Tekla.Engine.Domain;
public class Prim
{
    public string Name { get; set; }

    public Prim(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

public class Class : Prim
{
    public string Type { get; set; }

    public Class(string name, string type) : base(name)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}

public class Def : Prim
{
    public string Type { get; set; }
    public ComponentJson Component { get; set; }

    public Def(string name, string type, ComponentJson component) : base(name)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}

public class Over : Prim
{
    public ComponentJson Component { get; set; }

    public Over(string name, ComponentJson component) : base(name)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}