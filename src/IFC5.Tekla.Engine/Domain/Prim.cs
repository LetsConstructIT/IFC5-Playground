using IFC5Tekla.Engine.Models;
using System;
using System.Diagnostics;

namespace IFC5Tekla.Engine.Domain;

[DebuggerDisplay("{Name}")]
public class Prim
{
    public string Name { get; }

    public Prim(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

public class Class : Prim
{
    public string Type { get; }
    public string[] Inherits { get; }

    public Class(string name, string[] inherits, string type) : base(name)
    {
        Inherits = inherits ?? throw new ArgumentNullException(nameof(inherits));
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}

public class Def : Prim
{
    public string Type { get; }
    public ComponentJson Component { get; }
    public string[] Inherits { get; }

    public Def(string name, string[] inherits, string type, ComponentJson component) : base(name)
    {
        Inherits = inherits ?? throw new ArgumentNullException(nameof(inherits));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}

public class Over : Prim
{
    public ComponentJson Component { get; }

    public Over(string name, ComponentJson component) : base(name)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}